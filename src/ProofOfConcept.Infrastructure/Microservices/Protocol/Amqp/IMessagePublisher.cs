namespace ProofOfConcept.Infrastructure.Microservices.Protocol.Amqp;

public interface IMessagePublisher
{
    void Publish(string routingKey, string exchangeName, string body);
}