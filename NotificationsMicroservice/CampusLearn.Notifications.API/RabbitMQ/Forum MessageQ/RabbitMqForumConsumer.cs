using CampusLearn.Code.Library.RabbitMQ.ForumRabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace CampusLearn.Notifications.API.RabbitMQ.Forum_MessageQ;

public class RabbitMqForumConsumer
{

    private const string HostName = "rabbitmq";
    private const string ExchangeName = "forum_fanout";
    private const string QueueName = "forum_queue";

    public static async Task StartAsync()
    {
        var factory = new ConnectionFactory
        {
            HostName = HostName,
            UserName = "guest",
            Password = "guest"
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        // 1️. Declare exchange and queue
        await channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Fanout, durable: true);
        await channel.QueueDeclareAsync(QueueName, durable: true, exclusive: false, autoDelete: false);
        await channel.QueueBindAsync(QueueName, ExchangeName, "");

        // 2️. Set up async consumer
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var messageJson = Encoding.UTF8.GetString(body);

            // Deserialize message
            var message = JsonSerializer.Deserialize<NewForumMessage>(messageJson);

            if (message != null)
            { 
                // 🔹 Do something with the message
                InternalForumQueue.Messages.Enqueue(message);
                // e.g., save to another DB, trigger email, etc.
            }

            // 3️. Acknowledge the message
            await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
        };

        // 4️. Start consuming
        await channel.BasicConsumeAsync(QueueName, autoAck: false, consumer);

        Console.WriteLine("[Notifications API] Listening for messages...");

        // Keep running
        await Task.Delay(Timeout.Infinite);
    }
}
