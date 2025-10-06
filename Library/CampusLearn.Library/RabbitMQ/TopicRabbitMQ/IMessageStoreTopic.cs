namespace CampusLearn.Code.Library.RabbitMQ.TopicRabbitMQ;

//retrieves and stores messages
public interface IMessageStoreTopic
{
    Task<List<NewTopicMessage>> GetTopicMessagesAsync();
    Task AddTopicMessageAsync(NewTopicMessage message);
}
