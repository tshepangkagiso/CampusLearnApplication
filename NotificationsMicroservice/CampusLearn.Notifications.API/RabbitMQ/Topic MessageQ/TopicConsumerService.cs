namespace CampusLearn.Notifications.API.RabbitMQ;

public class TopicConsumerService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RabbitMqTopicConsumer.StartAsync();
    }
}
