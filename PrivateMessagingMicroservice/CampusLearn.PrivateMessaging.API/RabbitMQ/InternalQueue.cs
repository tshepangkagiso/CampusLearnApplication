using CampusLearn.Code.Library.RabbitMQ.TopicRabbitMQ;
using System.Collections.Concurrent;

namespace CampusLearn.PrivateMessaging.API.RabbitMQ;

public static class InternalQueue
{
    // Thread-safe queue for storing messages
    public static ConcurrentQueue<NewTopicMessage> Messages { get; } = new ConcurrentQueue<NewTopicMessage>();
}
