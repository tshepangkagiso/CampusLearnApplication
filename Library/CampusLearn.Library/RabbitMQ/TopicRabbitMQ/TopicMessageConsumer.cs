namespace CampusLearn.Code.Library.RabbitMQ.TopicRabbitMQ;

public class TopicMessageConsumer : IConsumer<NewTopicMessage>
{
    public Task Consume(ConsumeContext<NewTopicMessage> context)
    {
        return Task.CompletedTask;
    }
}
