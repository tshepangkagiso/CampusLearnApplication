using CampusLearn.Notifications.API.Services.Email;

namespace CampusLearn.Notifications.API.RabbitMQ.Forum_MessageQ;

public class ForumConsumerService(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        await RabbitMqForumConsumer.StartAsync(emailService);
    }
}

