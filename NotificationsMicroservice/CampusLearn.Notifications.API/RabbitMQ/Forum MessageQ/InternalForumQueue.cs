using CampusLearn.Code.Library.RabbitMQ.ForumRabbitMQ;
using System.Collections.Concurrent;

namespace CampusLearn.Notifications.API.RabbitMQ.Forum_MessageQ;

public static class InternalForumQueue
{
    // Thread-safe queue for storing messages
    public static ConcurrentQueue<NewForumMessage> Messages { get; } = new ConcurrentQueue<NewForumMessage>();
}
