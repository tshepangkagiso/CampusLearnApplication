namespace CampusLearn.UserProfileManagement.API.Controllers.Authentication.Services.Password;

public interface IHashingService
{
    (string?, int) HashPassword(string textPassword);
    bool VerifyPasswords(string textPassword, string hashedPassword);
}
