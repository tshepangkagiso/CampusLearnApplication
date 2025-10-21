namespace CampusLearn.UserProfileManagement.API.Controllers.Profile;

[Route("[controller]")]
[ApiController]
public class UserProfileController(UserManagementDbContext context, MinioService minio) : ControllerBase
{

    //route for updating user
    [HttpPut("update")]
    public async Task<IActionResult> OnUpdateUserProfile([FromForm] UpdateUserDTO request)
    {
        try
        {
            var oldUser = await context.UserProfiles.FirstOrDefaultAsync(x => x.UserProfileID == request.UserProfileID);
            if (oldUser == null) return NotFound("User not found");

   
            if (!string.IsNullOrEmpty(request.Email) && request.Email != oldUser.Email)
            {
                if (!request.Email.EndsWith("@student.belgiumcampus.ac.za") && !request.Email.EndsWith("@belgiumcampus.ac.za"))
                    return BadRequest("Invalid campus email format");

                var emailParts = request.Email.Split('@');
                if (emailParts.Length != 2)
                    return BadRequest("Invalid email format");

                var emailStudentNumber = emailParts[0];
                if (request.StudentNumber.ToString() != emailStudentNumber)
                    return BadRequest("Student number does not match email");

                // Check for duplicate email only if email is being changed
                var existingUser = await context.UserProfiles
                    .FirstOrDefaultAsync(u => u.Email == request.Email && u.UserProfileID != request.UserProfileID);
                if (existingUser != null)
                    return BadRequest("Email already exists");
            }


            oldUser.Name = string.IsNullOrEmpty(request.Name) ? oldUser.Name : request.Name;
            oldUser.Surname = string.IsNullOrEmpty(request.Surname) ? oldUser.Surname : request.Surname;
            oldUser.Email = string.IsNullOrEmpty(request.Email) ? oldUser.Email : request.Email;

            oldUser.StudentNumber = request.StudentNumber == 0 ? oldUser.StudentNumber : request.StudentNumber;

            oldUser.Qualification = Enum.IsDefined(typeof(Qualification), request.Qualification)
                ? request.Qualification : oldUser.Qualification;

            oldUser.UserRole = Enum.IsDefined(typeof(UserRole), request.UserRole)
                ? request.UserRole : oldUser.UserRole;

            // Handle profile picture upload
            if (request.ProfilePicture != null && request.ProfilePicture.Length > 0)
            {
                using var stream = request.ProfilePicture.OpenReadStream();
                var fileName = request.ProfilePicture.FileName;
                var contentType = request.ProfilePicture.ContentType;

                // Validate file type and size: these restriction are setting profile pic
                if (request.ProfilePicture.Length > 20 * 1024 * 1024) // 20MB limit
                    return BadRequest("File size too large");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(fileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest("Invalid file type");

                var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";

                await minio.UploadFileAsync(uniqueFileName, stream, contentType);

                oldUser.ProfilePictureUrl = uniqueFileName;

            }

            // Save changes
            context.UserProfiles.Update(oldUser);
            await context.SaveChangesAsync();

            return Ok(new { message = "User updated successfully" });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to update user");
            return BadRequest($"Failed to update user: {ex.Message}");
        }
    }


    //Retrieving file sent
    [HttpGet("file/{fileName}")]
    public async Task<IActionResult> GetFile(string fileName)
    {
        try
        {
            var (data, contentType) = await minio.OnRetrieveFile(fileName);

            // Return file with correct content type for display
            return File(data, contentType);
        }
        catch (Exception ex)
        {
            return NotFound($"File not found: {ex.Message}");
        }
    }


    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById([FromRoute] int userId)
    {
        try
        {
            var user = await context.UserProfiles
                .FirstOrDefaultAsync(u => u.UserProfileID == userId);

            if (user == null)
                return NotFound("User not found");

            return Ok(new
            {
                UserProfileID = user.UserProfileID,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                UserRole = user.UserRole,
                Qualification = user.Qualification,
                StudentNumber = user.StudentNumber,
                ProfilePictureUrl = user.ProfilePictureUrl,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLogin = user.LastLogin
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
            return BadRequest();
        }
    }

    [HttpGet("profile-picture-url/{userId}")]
    public async Task<IActionResult> GetUserProfilePicture([FromRoute] int userId)
    {
        try
        {
            var profilePicUrl = await context.UserProfiles
                .Where(u => u.UserProfileID == userId)
                .Select(u => new { u.ProfilePictureUrl })
                .FirstOrDefaultAsync();

            if (profilePicUrl == null)
                return NotFound("User not found");

            return Ok(new
            {
                UserProfileID = userId,
                ProfilePictureUrl = profilePicUrl.ProfilePictureUrl
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
            return BadRequest();
        }
    }

    [HttpGet("modules/{moduleCode}/assigned-tutor")]
    public async Task<IActionResult> GetRandomTutorForModule([FromRoute] string moduleCode)
    {
        try
        {
            // Get all tutors qualified for this module
            var qualifiedTutors = await context.TutorModules
                .Where(tm => tm.Module.ModuleCode == moduleCode && tm.IsActive)
                .Include(tm => tm.Tutor)
                .ThenInclude(t => t.UserProfile)
                .Select(tm => new
                {
                    tm.Tutor.TutorID,
                    tm.Tutor.UserProfile.Name,
                    tm.Tutor.UserProfile.Surname,
                    tm.Tutor.UserProfile.Email,
                    tm.QualifiedSince
                })
                .ToListAsync();

            if (!qualifiedTutors.Any())
            {
                return NotFound(new { Message = $"No tutors found for module {moduleCode}" });
            }

            // If only one tutor, return that one
            if (qualifiedTutors.Count == 1)
            {
                var tutor = qualifiedTutors[0];
                return Ok(new
                {
                    Message = "Only one tutor available",
                    Tutor = tutor
                });
            }

            // If multiple tutors, return random one
            var random = new Random();
            var randomIndex = random.Next(qualifiedTutors.Count);
            var selectedTutor = qualifiedTutors[randomIndex];

            return Ok(new
            {
                Message = $"Randomly selected from {qualifiedTutors.Count} tutors",
                Tutor = selectedTutor
            });
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to get random tutor for module {moduleCode}", ex);
            return BadRequest(new { Message = "Failed to get random tutor" });
        }
    }
}
