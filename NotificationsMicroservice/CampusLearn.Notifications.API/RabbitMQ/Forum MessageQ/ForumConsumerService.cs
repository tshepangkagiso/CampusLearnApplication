namespace CampusLearn.Notifications.API.RabbitMQ.Forum_MessageQ;

public class ForumConsumerService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RabbitMqForumConsumer.StartAsync();
    }
}

