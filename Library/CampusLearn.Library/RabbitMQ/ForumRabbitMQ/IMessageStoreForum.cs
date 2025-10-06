namespace CampusLearn.Code.Library.RabbitMQ.ForumRabbitMQ;

//retrieves and stores messages
public interface IMessageStoreForum
{
    Task<List<NewForumMessage>> GetForumMessagesAsync();
    Task AddForumMessageAsync(NewForumMessage message);
}
