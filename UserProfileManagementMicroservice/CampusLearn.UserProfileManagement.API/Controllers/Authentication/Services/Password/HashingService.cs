namespace CampusLearn.UserProfileManagement.API.Controllers.Authentication.Services.Password;

public class HashingService : IHashingService
{
    private const int SALT_ROUNDS = 12;

    //Hashing Method
    public (string?,int) HashPassword(string textPassword)
    {
        try
        {
            string hash = BCrypt.Net.BCrypt.HashPassword(textPassword, SALT_ROUNDS);
            int salt = SALT_ROUNDS;
            return (hash,salt);
        }
        catch(Exception ex)
        {
            Log.Logger.Error($"HashingService Error: {ex.Message.ToString()}");
            return (null,0);
        }
    }

    //Verifying Passwords
    public bool VerifyPasswords(string textPassword, string hashedPassword)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(textPassword, hashedPassword);
        }
        catch (Exception ex)
        {
            Log.Logger.Error($"HashingService Error: {ex.Message.ToString()}");
            return false;
        }
    }
}
