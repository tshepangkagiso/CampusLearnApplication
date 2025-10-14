using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace CampusLearn.Notifications.API.Services.Email;

public class EmailService(IOptions<SmtpSettings> options) : IEmailService
{
    private readonly SmtpSettings smtpSettings = options.Value;

    public async Task SendEmailAsync(string toSenderEmail,string emailSubject,string emailBody)
    {
        try
        {
            var fromSenderEmail = "tshepangkagisomashigo8@outlook.com";
            using var message = new MailMessage(fromSenderEmail, toSenderEmail, emailSubject, emailBody);
            using var emailClient = new SmtpClient(smtpSettings.HOST, smtpSettings.PORT)
            {
                Credentials = new NetworkCredential(smtpSettings.LOGIN, smtpSettings.MASTERKEY),
                EnableSsl = smtpSettings.ENABLESSL
            };

            await emailClient.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to send email: {ex.Message.ToString()}", ex);
            Console.WriteLine($"Failed to send email: {ex.Message.ToString()}");
        }
    }
}
