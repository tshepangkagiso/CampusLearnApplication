namespace CampusLearn.Code.Library.RabbitMQ.ForumRabbitMQ;

public class MessageStoreForum : IMessageStoreForum
{
    private readonly ConcurrentBag<NewForumMessage> _topicMessages = new();

    public Task AddForumMessageAsync(NewForumMessage message)
    {
        _topicMessages.Add(message);
        return Task.CompletedTask;
    }

    public Task<List<NewForumMessage>> GetForumMessagesAsync()
    {
        return Task.FromResult(_topicMessages.ToList());
    }
}
