namespace CampusLearn.TopicsManagement.API.RabbitMQ;

public class RabbitMqPublisher
{
    private const string exchangeName = "topics_fanout";

    private readonly ConnectionFactory factory;

    public RabbitMqPublisher()
    {
        factory = new ConnectionFactory { HostName = "rabbitmq", UserName = "guest", Password = "guest" };
    }

    public async Task<string> Publish<T>(T message)
    {
        await using var connection = await factory.CreateConnectionAsync(); 
        await using var channel = await connection.CreateChannelAsync(); 
        // Declare the fanout exchange
        await channel.ExchangeDeclareAsync( exchange: exchangeName, type: ExchangeType.Fanout, durable: true ); 
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)); 
        await channel.BasicPublishAsync( exchange: exchangeName, routingKey: string.Empty, body: body ); 
        return $"[Publisher] Sent: {DateTime.UtcNow}"; 
    }
}
