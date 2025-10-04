namespace CampusLearn.Code.Library.RabbitMQ.ForumRabbitMQ;

public class ForumMessagePublisher(IPublishEndpoint publishEndpoint) : IForumMessagePublisher
{
    public async Task PublishNewForumMessageAsync(NewForumMessage message)
    {
        await publishEndpoint.Publish(message);
    }
}
