namespace CampusLearn.Code.Library.RabbitMQ.ForumRabbitMQ;

//rabbitmq consuming message
public class ForumMessageConsumer : IConsumer<NewForumMessage>
{
    private readonly IMessageStoreForum _messageStore;
    private readonly ILogger<ForumMessageConsumer> _logger;

    public ForumMessageConsumer(IMessageStoreForum messageStore, ILogger<ForumMessageConsumer> logger)
    {
        _messageStore = messageStore;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<NewForumMessage> context)
    {
        var message = context.Message;
        _logger.LogInformation($"Received title : {message.Title} , Received message: {message.ModuleCode}");

        await _messageStore.AddForumMessageAsync(message);
    }
}
