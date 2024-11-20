using Newtonsoft.Json.Linq;

namespace ProofOfConcept.Infrastructure.Microservices.Protocol.Amqp;

public readonly record struct AmqpMessage(string TopicName, JToken Body);