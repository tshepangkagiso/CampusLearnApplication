using CampusLearn.Code.Library.RabbitMQ.TopicRabbitMQ;
using System.Collections.Concurrent;

namespace CampusLearn.Notifications.API.RabbitMQ;

public static class InternalTopicQueue
{
    // Thread-safe queue for storing messages
    public static ConcurrentQueue<NewTopicMessage> Messages { get; } = new ConcurrentQueue<NewTopicMessage>();
}
