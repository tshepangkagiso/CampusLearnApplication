namespace CampusLearn.UserProfileManagement.API.Controllers.AuthenticationControllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManagementDbContext context;
    private readonly IHashingService hashingService;
    private readonly IJwtService jwtService;

    public AuthController(UserManagementDbContext context, IHashingService hashingService, IJwtService jwtService)
    {
        this.context = context;
        this.hashingService = hashingService;
        this.jwtService = jwtService;
        
    }


    //http://localhost:6000/auth/register
    //route handles registration of a user
    [HttpPost("register")]
    public async Task<IActionResult> OnRegister([FromBody] RegisterRequest request)
    {
        try
        {
            // 1. Validate email format and student number match
            var emailParts = request.Email.Split('@');
            if (emailParts.Length != 2 || !request.Email.EndsWith("@student.belgiumcampus.ac.za"))
                return BadRequest("Invalid campus email format");

            var emailStudentNumber = emailParts[0];
            if (request.StudentNumber.ToString() != emailStudentNumber)
                return BadRequest("Student number does not match email");

            // 2. Validate enum values exist
            if (!Enum.IsDefined(typeof(UserRole), request.UserRole))
                return BadRequest("Invalid user role");

            if (!Enum.IsDefined(typeof(Qualification), request.Qualification))
                return BadRequest("Invalid qualification");

            // 3. Check if user already exists
            if (await this.context.UserProfiles.AnyAsync(u => u.Email == request.Email))
                return BadRequest("User already exists");

            // 4. Create UserProfile
            var userProfile = new UserProfile
            {
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email,
                UserRole = request.UserRole, //takes int value of enum
                Qualification = request.Qualification, //takes int value of enum
                StudentNumber = request.StudentNumber,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            /*UserRole { Student = 0, Tutor = 1, Admin = 2 }*/
            /*Qualification { DIP = 0, BIT = 1, BCOM = 2 }*/

            this.context.UserProfiles.Add(userProfile);
            await this.context.SaveChangesAsync();

            // 5. Create Login with hashed password
            var (hash, salt) = this.hashingService.HashPassword(request.Password);
            var login = new Login
            {
                Email = request.Email,
                PasswordHash = hash,
                PasswordSalt = salt,
                UserProfileID = userProfile.UserProfileID,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            this.context.Logins.Add(login);
            await this.context.SaveChangesAsync();

            // 6. Create Student/Tutor record based on role
            if (request.UserRole == UserRole.Student)
            {
                var student = new Student { UserProfileID = userProfile.UserProfileID };
                this.context.Students.Add(student);
            }
            else if (request.UserRole == UserRole.Tutor)
            {
                var tutor = new Tutor { UserProfileID = userProfile.UserProfileID };
                this.context.Tutors.Add(tutor);
            }

            await this.context.SaveChangesAsync();

            return Ok(new { Message = "Registration successful", UserId = userProfile.UserProfileID });
        }
        catch(Exception ex)
        {
            Log.Logger.Error($"Auth: {ex.Message.ToString()}");
            return StatusCode(500, $"Something went wrong with server: {ex.Message.ToString()}");
        }
    }



    //http://localhost:6000/auth/login
    //route handles login
    [HttpPost("login")]
    public async Task<IActionResult> OnLogin([FromBody] LoginRequest request)
    {
        try
        {
            // 1. Validate email format
            if (!request.Email.EndsWith("@student.belgiumcampus.ac.za"))
                return Unauthorized("Invalid campus email");

            // 2. Find user login
            var login = await this.context.Logins
                .Include(l => l.UserProfile)
                .FirstOrDefaultAsync(l => l.Email == request.Email);

            if (login == null || !login.IsActive)
                return Unauthorized("Invalid credentials");

            // 3. Check account lockout
            if (login.LockoutEnd.HasValue && login.LockoutEnd > DateTime.UtcNow)
                return Unauthorized($"Account locked until {login.LockoutEnd.Value}");

            // 4. Verify password
            if (!this.hashingService.VerifyPasswords(request.Password, login.PasswordHash))
            {
                // Failed attempt
                login.FailedLoginAttempts++;

                // Lock account after 5 failed attempts
                if (login.FailedLoginAttempts >= 5)
                    login.LockoutEnd = DateTime.UtcNow.AddMinutes(30);

                await this.context.SaveChangesAsync();
                return Unauthorized("Invalid credentials");
            }

            // 5. Successful login - reset security fields
            login.FailedLoginAttempts = 0;
            login.LockoutEnd = null;
            login.UserProfile.LastLogin = DateTime.UtcNow;
            await this.context.SaveChangesAsync();

            // 6. Generate JWT token (you'll need to implement IJwtService)
            var token = this.jwtService.GenerateToken(login.UserProfile);

            return Ok(new
            {
                Token = token,
                User = new
                {
                    login.UserProfile.UserProfileID,
                    login.UserProfile.Name,
                    login.UserProfile.Surname,
                    login.UserProfile.Email,
                    login.UserProfile.UserRole,
                    login.UserProfile.Qualification
                }
            });
        }
        catch (Exception ex)
        {
            Log.Logger.Error($"Auth: {ex.Message.ToString()}");
            return StatusCode(500, $"Something went wrong with server: {ex.Message.ToString()}");
        }
    }

    //http://localhost:6000/auth/validate-token
    //route handles login
    [HttpPost("validate-token")]
    public async Task<IActionResult> OnValidate([FromBody] ValidateTokenRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Token))
                return BadRequest("Token is required");

            bool isValid = this.jwtService.ValidateToken(request.Token);

            if (!isValid)
                return Unauthorized(new 
                { 
                    Valid = false, 
                    Message = "Token is invalid or expired",
                    User = new
                    {
                        UserId = "",
                        Email = "",
                        Role = ""
                    }
                });

            // Get user info from token for the response
            var principal = this.jwtService.GetPrincipalFromToken(request.Token);
            var userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmail = principal?.FindFirst(ClaimTypes.Email)?.Value;
            var userRole = principal?.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                Valid = true,
                Message = "Token is valid",
                User = new
                {
                    UserId = userId,
                    Email = userEmail,
                    Role = userRole
                }
            });
        }
        catch (Exception ex)
        {
            Log.Logger.Error($"Auth: {ex.Message.ToString()}");
            return StatusCode(500, $"Something went wrong with server: {ex.Message.ToString()}");
        }
    }

}



/*
         try
        {
            return BadRequest();
        }
        catch(Exception ex)
        {
            Log.Logger.Error($"Auth: {ex.Message.ToString()}");
            return StatusCode(500, "Something went wrong with server");
        }

*/

