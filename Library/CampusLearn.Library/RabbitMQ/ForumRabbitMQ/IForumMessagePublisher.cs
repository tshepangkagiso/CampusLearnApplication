namespace CampusLearn.Code.Library.RabbitMQ.ForumRabbitMQ;

public interface IForumMessagePublisher
{
    Task PublishNewForumMessageAsync(NewForumMessage message);
}
