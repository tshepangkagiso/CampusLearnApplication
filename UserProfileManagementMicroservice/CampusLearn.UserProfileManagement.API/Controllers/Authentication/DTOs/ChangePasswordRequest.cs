namespace CampusLearn.UserProfileManagement.API.Controllers.Authentication.DTOs;

public class ChangePasswordRequest
{
    public int UserID { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
