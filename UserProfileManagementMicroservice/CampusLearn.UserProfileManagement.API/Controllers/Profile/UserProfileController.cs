namespace CampusLearn.UserProfileManagement.API.Controllers.Profile;

[Route("[controller]")]
[ApiController]
public class UserProfileController(UserManagementDbContext context, MinioService minio) : ControllerBase
{
    //route for updating user
    [HttpPost("update")]
    public async Task<IActionResult> OnUpdateUserProfile([FromForm] UpdateUserDTO request)
    {
        try
        {
            var oldUser = await context.UserProfiles.FirstOrDefaultAsync(x => x.UserProfileID == request.UserProfileID);
            if (oldUser == null) return NotFound("User not found");

            // Fix email validation logic
            if (!request.Email.EndsWith("@student.belgiumcampus.ac.za") && !request.Email.EndsWith("@belgiumcampus.ac.za"))
                return BadRequest("Invalid campus email format");

            var emailParts = request.Email.Split('@');
            if (emailParts.Length != 2)
                return BadRequest("Invalid email format");

            var emailStudentNumber = emailParts[0];
            if (request.StudentNumber.ToString() != emailStudentNumber)
                return BadRequest("Student number does not match email");

            if (!Enum.IsDefined(typeof(UserRole), request.UserRole))
                return BadRequest("Invalid user role");

            if (!Enum.IsDefined(typeof(Qualification), request.Qualification))
                return BadRequest("Invalid qualification");

            // Check for duplicate email (excluding current user)
            var existingUser = await context.UserProfiles
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.UserProfileID != request.UserProfileID);
            if (existingUser != null)
                return BadRequest("Email already exists");

            // Handle profile picture upload
            if (request.ProfilePicture != null && request.ProfilePicture.Length > 0)
            {
                using var stream = request.ProfilePicture.OpenReadStream();
                var fileName = request.ProfilePicture.FileName;
                var contentType = request.ProfilePicture.ContentType;

                // Validate file type and size: these restriction are setting profile pic
                if (request.ProfilePicture.Length > 5 * 1024 * 1024) // 5MB limit
                    return BadRequest("File size too large");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(fileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest("Invalid file type");

                var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";

                await minio.UploadFileAsync(uniqueFileName, stream, contentType);

                oldUser.ProfilePictureUrl = uniqueFileName;

            }

            // Update user properties
            oldUser.Name = request.Name;
            oldUser.Surname = request.Surname;
            oldUser.Email = request.Email;
            oldUser.StudentNumber = request.StudentNumber;
            oldUser.Qualification = request.Qualification;
            oldUser.UserRole = request.UserRole;

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

    //testing routes:


    [HttpGet("buckets")]
    public async Task<IActionResult> GetBuckets()
    {
        try
        {
            var buckets = await minio.ListBucketsAsync();
            return Ok(buckets);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to get buckets");
            return BadRequest($"Failed to get buckets: {ex.Message}");
        }
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        try
        {
            Console.WriteLine($"Upload request received. File: {file?.FileName}, Size: {file?.Length}");

            if (file == null || file.Length == 0)
            {
                Console.WriteLine("No file or empty file provided");
                return BadRequest("No file provided");
            }

            // Validate file size
            if (file.Length > 100 * 1024 * 1024) // 100MB limit
            {
                Console.WriteLine($"File size {file.Length} exceeds limit");
                return BadRequest("File size exceeds limit");
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            Console.WriteLine($"Generated filename: {uniqueFileName}");

            using Stream data = file.OpenReadStream();
            Console.WriteLine($"Stream created. CanRead: {data.CanRead}, Length: {data.Length}");

            await minio.UploadFileAsync(uniqueFileName, data, file.ContentType);
            Console.WriteLine("File uploaded successfully");
            return Ok(new
            {
                Message = "File successfully uploaded",
                FileName = uniqueFileName,
                OriginalName = file.FileName,
                Size = file.Length
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Controller exception: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            return BadRequest($"Failed to upload file: {ex.Message}");
        }
    }
}
