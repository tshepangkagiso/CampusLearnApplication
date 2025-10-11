namespace CampusLearn.UserProfileManagement.API.Controllers.Profile.DTOs;

public class UpdateUserDTO
{

    public int UserProfileID { get; set; }
    public UserRole UserRole { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IFormFile? ProfilePicture { get; set; }
    public Qualification Qualification { get; set; }
    public int StudentNumber { get; set; }
}
