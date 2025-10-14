namespace CampusLearn.Notifications.API.Services.Email;

public class SmtpSettings 
{
    public string HOST { get; set; } = string.Empty;
    public int PORT { get; set; } 
    public string LOGIN { get; set; } = string.Empty;
    public string MASTERKEY { get; set; } = string.Empty;
    public bool ENABLESSL { get; set; }
}
