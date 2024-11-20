using ProofOfConcept.Core.Specifications;

namespace ProofOfConcept.Infrastructure.Microservices.Protocol.Amqp;

public static class AmqpModelExtensions
{
    public static List<AmqpConfiguration> GetAmqpConfiguration(this Specification specification)
    {
        var exchangeName = specification.InitialVariables.FirstOrDefault(x => x.Name.Equals("exchangeName")).Value;
        var queueName = specification.InitialVariables.FirstOrDefault(x => x.Name.Equals("queueName")).Value;

        // TODO: We should do variable resolving here, instead of just supporting strings
        return specification.Switches
            .Where(x => x.Gate.Label.Equals("!amqp_publish"))
            .Select(x => x.Guards.FirstOrDefault(x => x.LeftOperand == "topicName"))
            .Select(x => new AmqpConfiguration(queueName, exchangeName, x.RightOperand))
            .Distinct()
            .ToList();
    }
}