namespace CampusLearn.Code.Library.RabbitMQ.TopicRabbitMQ;


//rabbitmq consuming message
public class TopicMessageConsumer : IConsumer<NewTopicMessage>
{
    private readonly IMessageStoreTopic _messageStore;
    private readonly ILogger<TopicMessageConsumer> _logger;

    public TopicMessageConsumer(IMessageStoreTopic messageStore, ILogger<TopicMessageConsumer> logger)
    {
        _messageStore = messageStore;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<NewTopicMessage> context)
    {
        var message = context.Message;
        _logger.LogInformation($"Received title : {message.Title} , Received message: {message.ModuleCode}" );

        await _messageStore.AddTopicMessageAsync(message);
    }
}
