namespace CampusLearn.Code.Library.RabbitMQ.TopicRabbitMQ;

public interface ITopicMessagePublisher
{
    Task PublishNewTopicMessageAsync(NewTopicMessage message);
}
