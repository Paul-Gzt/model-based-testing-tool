namespace ProofOfConcept.Infrastructure.Microservices.Protocol.Amqp;

public readonly record struct AmqpConfiguration(string QueueName, string ExchangeName, string TopicName);