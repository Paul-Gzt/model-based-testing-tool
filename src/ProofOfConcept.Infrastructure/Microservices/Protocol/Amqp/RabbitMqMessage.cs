using Newtonsoft.Json.Linq;

namespace ProofOfConcept.Infrastructure.Microservices.Protocol.Amqp;

public readonly record struct RabbitMqMessage(JToken Data);