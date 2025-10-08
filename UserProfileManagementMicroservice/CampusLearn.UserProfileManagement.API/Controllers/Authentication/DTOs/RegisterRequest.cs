namespace CampusLearn.UserProfileManagement.API.Controllers.Authentication.DTOs;

public class RegisterRequest
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserRole UserRole { get; set; }
    public Qualification Qualification { get; set; }
    public int StudentNumber { get; set; }
}
