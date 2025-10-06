namespace CampusLearn.Code.Library.RabbitMQ.TopicRabbitMQ;

public class MessageStoreTopic : IMessageStoreTopic
{
    private readonly ConcurrentBag<NewTopicMessage> _topicMessages = new();

    public Task AddTopicMessageAsync(NewTopicMessage message)
    {
        _topicMessages.Add(message);
        return Task.CompletedTask;
    }

    public Task<List<NewTopicMessage>> GetTopicMessagesAsync()
    {
        return Task.FromResult(_topicMessages.ToList());
    }
}