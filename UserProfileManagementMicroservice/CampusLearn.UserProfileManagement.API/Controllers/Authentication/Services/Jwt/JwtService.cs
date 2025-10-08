namespace CampusLearn.UserProfileManagement.API.Controllers.Authentication.Services.Jwt;

public class JwtService : IJwtService
{
    private readonly string secret;
    private readonly string issuer;
    private readonly string audience;

    public JwtService(IConfiguration config)
    {
        this.secret = config["Jwt:Secret"];
        this.issuer = config["Jwt:Issuer"];
        this.audience = config["Jwt:Audience"];
    }

    public string GenerateToken(UserProfile user)
    {
        var claims = new[]
        {
            // Core Identity Claims
            new Claim(JwtRegisteredClaimNames.Sub, user.UserProfileID.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            
            // Application-Specific Claims
            new Claim(ClaimTypes.NameIdentifier, user.UserProfileID.ToString()),
            new Claim(ClaimTypes.Name, $"{user.Name} {user.Surname}"),
            new Claim(ClaimTypes.Role, user.UserRole.ToString()), // "Student", "Tutor", "Admin"
            
            // CampusLearn Specific Claims
            new Claim("UserProfileID", user.UserProfileID.ToString()),
            new Claim("StudentNumber", user.StudentNumber.ToString()),
            new Claim("Qualification", user.Qualification.ToString()), // "DIP", "BIT", "BCOM"
            new Claim("FullName", $"{user.Name} {user.Surname}"),
            new Claim("IsActive", user.IsActive.ToString()),
            
            // Role-specific flags (for quick access)
            new Claim("IsStudent", (user.UserRole == UserRole.Student).ToString()),
            new Claim("IsTutor", (user.UserRole == UserRole.Tutor).ToString()),
            new Claim("IsAdmin", (user.UserRole == UserRole.Admin).ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: this.issuer,
            audience: this.audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2), // 2-hour token lifetime
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public bool ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(this.secret);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = this.issuer,
                ValidateAudience = true,
                ValidAudience = this.audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(this.secret);

            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = this.issuer,
                ValidateAudience = true,
                ValidAudience = this.audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return principal;
        }
        catch
        {
            return null;
        }
    }
}
