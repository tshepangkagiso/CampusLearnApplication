
namespace CampusLearn.UserProfileManagement.API.Controllers.Authentication.Services.Jwt;

public interface IJwtService
{
    string GenerateToken(UserProfile user);
    ClaimsPrincipal GetPrincipalFromToken(string token);
    bool ValidateToken(string token);
}
