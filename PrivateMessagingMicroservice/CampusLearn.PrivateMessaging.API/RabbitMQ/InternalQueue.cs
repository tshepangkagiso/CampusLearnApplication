using CampusLearn.Code.Library.RabbitMQ.TopicRabbitMQ;
using System.Collections.Concurrent;

namespace CampusLearn.PrivateMessaging.API.RabbitMQ;

public static class InternalQueue
{
    // Thread-safe queue for storing messages
    public static ConcurrentQueue<NewTopicMessage> Messages { get; } = new ConcurrentQueue<NewTopicMessage>();
}
/*
    Fanout Exchange → If multiple consumers are bound to the same exchange with different queues, each gets its own copy of the message.

    Manual Ack (autoAck: false) → ensures RabbitMQ won’t remove the message until you confirm processing succeeded.
 */