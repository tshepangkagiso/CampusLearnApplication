using System.Security.Cryptography;

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
    public async Task<IActionResult> OnRegister([FromBody] Authentication.DTOs.RegisterRequest request)
    {
        try
        {
            // 1. Validate email format and student number match
            var emailParts = request.Email.Split('@');
            if (emailParts.Length != 2 || !request.Email.EndsWith("@student.belgiumcampus.ac.za") && !request.Email.EndsWith("@belgiumcampus.ac.za"))
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
    public async Task<IActionResult> OnLogin([FromBody] Authentication.DTOs.LoginRequest request)
    {
        try
        {
            // 1. Validate email format
            if (!request.Email.EndsWith("@student.belgiumcampus.ac.za") && !request.Email.EndsWith("@belgiumcampus.ac.za"))
                return Unauthorized("Invalid campus email.");

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



    [HttpPost("reset-password")]
    public async Task<IActionResult> OnResetPassword([FromBody] Authentication.DTOs.ResetPasswordRequest request)
    {
        try
        {
            // Validate email format
            if (!request.Email.EndsWith("@student.belgiumcampus.ac.za"))
                return BadRequest("Invalid campus email");

            // Find user login
            var login = await context.Logins
                .Include(l => l.UserProfile)
                .FirstOrDefaultAsync(l => l.Email == request.Email);

            if (login == null || !login.IsActive)
                return BadRequest("User not found");

            string temporaryPassword = GeneratePassword();
            // Hash new password
            var (hash, salt) = hashingService.HashPassword(temporaryPassword);

            // Update password
            login.PasswordHash = hash;
            login.PasswordSalt = salt;
            login.LastPasswordChange = DateTime.UtcNow;
            login.FailedLoginAttempts = 0; // Reset failed attempts
            login.LockoutEnd = null; // Remove any lockout

            await context.SaveChangesAsync();

            return Ok(new { password = temporaryPassword });
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
            return BadRequest("Error resetting password");
        }
    }

    [HttpPost("change-password")] //inside the application
    public async Task<IActionResult> OnChangePassword([FromBody] Authentication.DTOs.ChangePasswordRequest request)
    {
        try
        {
            // Get current user from JWT token
            var userId = request.UserID;

            var login = await context.Logins
                .FirstOrDefaultAsync(l => l.UserProfileID == userId);

            if (login == null || !login.IsActive)
                return BadRequest("User not found");

            // Verify current password
            if (!hashingService.VerifyPasswords(request.CurrentPassword, login.PasswordHash))
                return BadRequest("Current password is incorrect");

            // Hash new password
            var (hash, salt) = hashingService.HashPassword(request.NewPassword);

            // Update password
            login.PasswordHash = hash;
            login.PasswordSalt = salt;
            login.LastPasswordChange = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return Ok(new { Message = "Password changed successfully" });
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
            return BadRequest("Error changing password");
        }
    }





    private string GeneratePassword(int length = 12)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";

        byte[] data = new byte[length];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(data);
        }

        char[] password = new char[length];
        for (int i = 0; i < length; i++)
        {
            password[i] = chars[data[i] % chars.Length];
        }

        return new string(password);
    }

}




