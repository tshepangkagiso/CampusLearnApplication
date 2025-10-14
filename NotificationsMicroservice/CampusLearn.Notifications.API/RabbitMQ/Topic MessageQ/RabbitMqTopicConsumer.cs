using CampusLearn.Code.Library.RabbitMQ.TopicRabbitMQ;
using CampusLearn.Notifications.API.Services.Email;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace CampusLearn.Notifications.API.RabbitMQ;
public class RabbitMqTopicConsumer
{
    private const string HostName = "rabbitmq"; 
    private const string ExchangeName = "topics_fanout";
    private const string QueueName = "topic_queue_a";

    public static async Task StartAsync(IEmailService emailService)
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
            var message = JsonSerializer.Deserialize<NewTopicMessage>(messageJson);

            // 🔹 Do something with the message
            if (message != null)
            {
                InternalTopicQueue.Messages.Enqueue(message);

                string to = "578012@student.belgiumcampus.ac.za";
                string emailSubject = message.Title;
                string emaiBody = $"New topic has been created for the module you subscribed too with code: {message.ModuleCode}";

                await emailService.SendEmailAsync(to, emailSubject, emaiBody);
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

/*
    Fanout Exchange → If multiple consumers are bound to the same exchange with different queues, each gets its own copy of the message.

    Manual Ack (autoAck: false) → ensures RabbitMQ won’t remove the message until you confirm processing succeeded.
 */