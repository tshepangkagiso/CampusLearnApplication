using CampusLearn.PrivateMessaging.API.Signal_R.ChatRoomService;
namespace CampusLearn.PrivateMessaging.API.RabbitMQ;

public class TopicConsumerService(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        var chatRoomService = scope.ServiceProvider.GetRequiredService<IChatRoomService>();
        await RabbitMqConsumer.StartAsync(chatRoomService);
    }
}
