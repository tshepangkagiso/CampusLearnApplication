namespace CampusLearn.Code.Library.RabbitMQ.ForumRabbitMQ;

public class ForumMessageConsumer : IConsumer<NewForumMessage>
{
    public Task Consume(ConsumeContext<NewForumMessage> context)
    {
        return Task.CompletedTask;
    }
}
