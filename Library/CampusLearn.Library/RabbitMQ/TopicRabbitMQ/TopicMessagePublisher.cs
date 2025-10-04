namespace CampusLearn.Code.Library.RabbitMQ.TopicRabbitMQ;

public class TopicMessagePublisher(IPublishEndpoint publishEndpoint) : ITopicMessagePublisher
{
    public async Task PublishNewTopicMessageAsync(NewTopicMessage message)
    {
        await publishEndpoint.Publish(message);
    }
}
