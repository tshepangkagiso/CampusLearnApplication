namespace CampusLearn.PrivateMessaging.API.RabbitMQ;

public class TopicConsumerService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RabbitMqConsumer.StartAsync();
    }
}
