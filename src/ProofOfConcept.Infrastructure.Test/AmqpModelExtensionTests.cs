using System.Collections.Immutable;
using FluentAssertions;
using ProofOfConcept.Core.Solver;
using ProofOfConcept.Core.Specifications;
using ProofOfConcept.Infrastructure.Microservices.Protocol.Amqp;

namespace ProofOfConcept.Infrastructure.Test;

[TestClass]
public class AmqpModelExtensionTests
{
    [TestMethod]
    public void Empty_Specification_Does_Nothing()
    {
        // Arrange 
        var specification = new Specification();

        // Act
        var result = specification.GetAmqpConfiguration();

        // Assert
        result.Should().BeEmpty();
    }

    [TestMethod]
    public void NoAmqp_Specification_Does_Nothing()
    {
        // Arrange 
        var specification = new Specification
        {
            Gates = new List<Gate>
            {
                new(ActionType.Input, "?amqp_receive"),
                new(ActionType.Input, "!http_ok")
            }
        };

        // Act
        var result = specification.GetAmqpConfiguration();

        // Assert
        result.Should().BeEmpty();
    }

    [TestMethod]
    public void Amqp_In_Specification_Does_Returns()
    {
        // Arrange 
        var specification = new Specification
        {
            InitialVariables = new List<Variable>
            {
                new Variable("queueName", "myqueue", typeof(string)),
                new Variable("exchangeName", "myexchange", typeof(string))
            }.ToImmutableList(),
            Gates = new List<Gate>
            {
                new(ActionType.Input, "?amqp_receive"),
                new(ActionType.Input, "!amqp_publish")
            },
            Switches = new List<Switch>
            {
                new(new Location("l0"), 
                    new Gate(ActionType.Input, "!amqp_publish"), 
                    new List<Guard>
                    {
                        new ("topicName", Operation.Equals, "orders-confirmed")
                    }, 
                    null, 
                    new Location("l1")),
                new(new Location("l0"),
                    new Gate(ActionType.Input, "!amqp_publish"),
                    new List<Guard>
                    {
                        new("topicName", Operation.Equals, "orders-received")
                    },
                    null,
                    new Location("l2"))
            }
        };

        var expected = new List<AmqpConfiguration>
        {
            new("myqueue", "myexchange", "orders-confirmed"),
            new("myqueue", "myexchange", "orders-received")
        };

        // Act
        var result = specification.GetAmqpConfiguration();

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(expected);

    }
}