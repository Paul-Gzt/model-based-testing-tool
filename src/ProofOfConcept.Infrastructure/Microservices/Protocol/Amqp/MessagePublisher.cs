using System.Text;
using RabbitMQ.Client;

namespace ProofOfConcept.Infrastructure.Microservices.Protocol.Amqp;

public class MessagePublisher : IMessagePublisher
{
    public void Publish(string routingKey, string exchangeName, string body)
    {
        using var channel = CreateChannel();

        var properties = channel.CreateBasicProperties();
        properties.ContentType = "application/json";
        properties.Headers = new Dictionary<string, object>
        {
            {"TestingFramework", true}
        };

        var messageBody = Encoding.UTF8.GetBytes(body);
        channel.BasicPublish(exchange: exchangeName,
            routingKey: routingKey,
            basicProperties: properties,
            body: messageBody);

        Console.WriteLine($"Sending to exchangeName '{exchangeName}', routingKey: '{routingKey}' with body: '{body}'");
        Task.Delay(TimeSpan.FromSeconds(2)).GetAwaiter().GetResult();
    }

    private static IModel? CreateChannel()
    {
        var factory = new ConnectionFactory {HostName = "localhost"};
        var connection = factory.CreateConnection();
        return connection.CreateModel();
    }
}