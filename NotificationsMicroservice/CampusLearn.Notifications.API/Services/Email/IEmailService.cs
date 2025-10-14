
namespace CampusLearn.Notifications.API.Services.Email;

public interface IEmailService
{
    Task SendEmailAsync(string toSenderEmail, string emailSubject, string emailBody);
}