using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProofOfConcept.Core.Specifications;
using ProofOfConcept.Infrastructure.Microservices.Protocol.Amqp;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ProofOfConcept.Infrastructure.Microservices.Output;

public class AmqpOutputAdapter : OutputAdapterBase
{
    private readonly string _exchangeName;
    private readonly string _topicName;
    private readonly string _queueName;
    
    /// <summary>
    /// Creates the correct RabbitMQ bindings and purges any unread messages from previous runs.
    /// This is to reduce flakiness during testing.
    /// </summary>
    /// <param name="queueName">The name of the subscription for this testing framework</param>
    /// <param name="exchangeName">The name of the exchange where messages will be published to</param>
    /// <param name="topicName">The specific event we are interested in</param>
    public AmqpOutputAdapter(string queueName, string exchangeName, string topicName)
    {
        _queueName = queueName.Replace("\"", string.Empty);;
        _exchangeName = exchangeName.Replace("\"", string.Empty);;
        _topicName = topicName.Replace("\"", string.Empty);

        var channel = CreateChannel();

        channel?.QueuePurge(_queueName);
    }

    public override void ReceiveResponse(Trace? trace)
    {
        throw new NotImplementedException();
    }

    protected override List<Trace> Observe()
    {
        return GetMessages(TimeSpan.FromSeconds(1));
    }
    
    private List<Trace> GetMessages(TimeSpan timeToRead)
    {
        var traces = new List<Trace>();
        try
        {
            using var channel = CreateChannel();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += ReceiveMessage(traces);
            channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            Thread.Sleep(timeToRead);

            return traces;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error while fetching AMQP messages: {0}", e);
            return new List<Trace>();
        }
    }

    private EventHandler<BasicDeliverEventArgs> ReceiveMessage(List<Trace> traces)
    {
        return (_, eventArgs) =>
        {
            if (eventArgs.BasicProperties.Headers is not null && eventArgs.BasicProperties.Headers.TryGetValue("TestingFramework", out var _))
            {
                // Prevent consuming own messages, such that it has no effect in the model
                return;
            }

            var body = eventArgs.Body.ToArray();
            var messageBody = Encoding.UTF8.GetString(body);
            var messageAsJtoken = JToken.Parse(messageBody);
            var rabbitMqMessageBody = new RabbitMqMessage(messageAsJtoken);

            var amqpMessage = new AmqpMessage(eventArgs.RoutingKey, rabbitMqMessageBody.Data);
            
            traces.Add(new Trace(new Gate(ActionType.Output, "!amqp_publish"), JsonConvert.SerializeObject(amqpMessage)));
        };
    }

    private IModel? CreateChannel()
    {
        var factory = new ConnectionFactory {HostName = "localhost"};
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();
        
        channel.QueueDeclare(_queueName, true, false, false, null);
        channel.QueueBind(queue: _queueName, exchange: _exchangeName, routingKey: _topicName);
        
        return channel;
    }
}