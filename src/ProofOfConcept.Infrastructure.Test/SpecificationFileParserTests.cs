using FluentAssertions;
using ProofOfConcept.Core.Solver;
using ProofOfConcept.Core.Specifications;
using ProofOfConcept.Infrastructure.Sts;

namespace ProofOfConcept.Infrastructure.Test;

[TestClass]
public class SpecificationFileParserTests
{
    
    [TestMethod]
    public async Task ParsesSimpleModel()
    {
        // Arrange
        var file = await FileHelper.ReadFileAsync("Models", "SimpleModel.model");

        // Act
        var actual = SpecificationParser.ParseSpecification(file);

        // Assert
        actual.InitialLocation.Name.Should().Be("l0");
        actual.InitialVariables.First().Should().BeEquivalentTo(new Variable
        {
            Name = "x",
            Value = "0",
            Type = typeof(int)
        });
        actual.Switches.First().Should().BeEquivalentTo(new Switch
        (
            From: new Location("l0"),
            Gate: new Gate(ActionType.Input, "?inX"),
            Guards: new List<Guard>
            {
                new()
                {
                    LeftOperand = "x",
                    Operation = Operation.Equals,
                    RightOperand = "0"
                }
            },
            To: new Location("l1"),
            UpdateMapping: new UpdateMapping
            {
                AssociatedVariableName = "x",
                UpdateStatement = new UpdateStatement("x", Operation.Addition, "1")
            }
        ));
        actual.Switches.ElementAt(1).Should().BeEquivalentTo(new Switch
        (
            From: new Location("l1"),
            Gate: new Gate(ActionType.Output, "!outX"),
            Guards: new List<Guard>
            {
                new()
                {
                    LeftOperand = "x",
                    Operation = Operation.Equals,
                    RightOperand = "1"
                }
            },
            To: new Location("l2"),
            UpdateMapping: null
        ));
    }
    
    [TestMethod]
    public async Task ParsesVerboseSimpleModel()
    {
        // Arrange
        var file = await FileHelper.ReadFileAsync("Models", "VerboseSimpleModel.model");

        // Act
        var actual = SpecificationParser.ParseSpecification(file);
        
        // Assert
        actual.InitialLocation.Name.Should().Be("l0");
        actual.InitialVariables.First().Should().BeEquivalentTo(new Variable
        {
            Name = "x",
            Value = "0",
            Type = typeof(int)
        });
        actual.Switches.First().Should().BeEquivalentTo(new Switch
        (
            From: new Location("l0"),
            Gate: new Gate(ActionType.Input, "?inX"),
            Guards: new List<Guard>
            {
                new()
                {
                    LeftOperand = "x",
                    Operation = Operation.Equals,
                    RightOperand = "0"
                }
            },
            To: new Location("l1"),
            UpdateMapping: new UpdateMapping
            {
                AssociatedVariableName = "x",
                UpdateStatement = new UpdateStatement("x", Operation.Addition, "1")
            }
        ));
        actual.Switches.ElementAt(1).Should().BeEquivalentTo(new Switch
        (
            From: new Location("l1"),
            Gate: new Gate(ActionType.Output, "!outX"),
            Guards: new List<Guard>
            {
                new()
                {
                    LeftOperand = "x",
                    Operation = Operation.Equals,
                    RightOperand = "1"
                }
            },
            To: new Location("l2"),
            UpdateMapping: null
        ));
    }
    
    [TestMethod]
    public async Task ParsesVerboseSimpleModelWithoutClauses()
    {
        // Arrange
        var file = await FileHelper.ReadFileAsync("Models", "VerboseSimpleModelWithoutClauses.model");

        // Act
        var actual = SpecificationParser.ParseSpecification(file);
        
        // Assert
        actual.InitialLocation.Name.Should().Be("l0");
        actual.InitialVariables.Should().BeEmpty();
        actual.Switches.Should().HaveCount(3);
        actual.Switches.First().Should().BeEquivalentTo(new Switch
        (
            From: new Location("l0"),
            Gate: new Gate(ActionType.Input, "?inX"),
            Guards: new List<Guard>(),
            To: new Location("l1"),
            UpdateMapping: null
        ));
        actual.Switches.ElementAt(1).Should().BeEquivalentTo(new Switch
        (
            From: new Location("l1"),
            Gate: new Gate(ActionType.Output, "!outX"),
            Guards: new List<Guard>(),
            To: new Location("l2"),
            UpdateMapping: null
        ));
        actual.Switches.ElementAt(2).Should().BeEquivalentTo(new Switch
        (
            From: new Location("l1"),
            Gate: new Gate(ActionType.Output, "!outX2"),
            Guards: new List<Guard>(),
            To: new Location("l3"),
            UpdateMapping: null
        ));
    }
    
     [TestMethod]
    public async Task ParsesSimpleModelWhereAndUpdateNull()
    {
        // Arrange
        var file = await FileHelper.ReadFileAsync("Models", "SimpleModelWhereAndUpdateNull.model");

        // Act
        var actual = SpecificationParser.ParseSpecification(file);

        // Assert
        actual.InitialLocation.Name.Should().Be("l0");
        actual.InitialVariables.First().Should().BeEquivalentTo(new Variable
        {
            Name = "x",
            Value = "0",
            Type = typeof(int)
        });
        actual.Switches.First().Should().BeEquivalentTo(new Switch
        (
            From: new Location("l0"),
            Gate: new Gate(ActionType.Input, "?inX"),
            Guards: new List<Guard>
            {
                new()
                {
                    LeftOperand = "p",
                    Operation = Operation.LessThan,
                    RightOperand = "10"
                }   
            },
            To: new Location("l1"),
            UpdateMapping: new UpdateMapping
            {
                AssociatedVariableName = "x",
                UpdateStatement = new UpdateStatement("x", Operation.Addition, "1")
            }
        ));
        actual.Switches.ElementAt(1).Should().BeEquivalentTo(new Switch
        (
            From: new Location("l1"),
            Gate: new Gate(ActionType.Output, "!outX"),
            Guards: new List<Guard>(),
            To: new Location("l2"),
            UpdateMapping: null
        ));
    }

    [TestMethod]
    public async Task ParsesSimpleModelMultipleGuards()
    {
        // Arrange
        var file = await FileHelper.ReadFileAsync("Models", "SimpleModelMultipleGuards.model");

        // Act
        var actual = SpecificationParser.ParseSpecification(file);

        // Assert
        actual.InitialLocation.Name.Should().Be("l0");
        actual.InitialVariables.First().Should().BeEquivalentTo(new Variable
        {
            Name = "x",
            Value = "0",
            Type = typeof(int)
        });
        actual.Switches.First().Should().BeEquivalentTo(new Switch
        (
            From: new Location("l0"),
            Gate: new Gate(ActionType.Input, "?inX"),
            Guards: new List<Guard>
            {
                new()
                {
                    LeftOperand = "x",
                    Operation = Operation.Equals,
                    RightOperand = "0"
                },
                new()
                {
                    LeftOperand = "y",
                    Operation = Operation.Equals,
                    RightOperand = "1"
                }
            },
            To: new Location("l1"),
            UpdateMapping: new UpdateMapping
            {
                AssociatedVariableName = "x",
                UpdateStatement = new UpdateStatement("x", Operation.Addition, "1")
            }
        ));
        actual.Switches.ElementAt(1).Should().BeEquivalentTo(new Switch
        (
            From: new Location("l1"),
            Gate: new Gate(ActionType.Output, "!outX"),
            Guards: new List<Guard>
            {
                new()
                {
                    LeftOperand = "x",
                    Operation = Operation.Equals,
                    RightOperand = "1"
                }
            },
            To: new Location("l2"),
            UpdateMapping: null
        ));
    }

    [TestMethod]
    public async Task ParsesComponents_To_Variables()
    {
        // Arrange
        var file = await FileHelper.ReadFileAsync("Models", "ModelWithVariables.model");

        // Act
        var actual = SpecificationParser.ParseSpecification(file);
        
        // Assert
        actual.InitialVariables.Should().BeEquivalentTo(new List<Variable>
        {
            new("x", "0", typeof(int)),
            new("weatherForecast", "https://localhost:7205/WeatherForecast", typeof(string)),
            new("static", "https://localhost:7205/static", typeof(string)),
            new("static2", "https://localhost:7205/static2", typeof(string)),
            new("post", "https://localhost:7205/post", typeof(string)),
            new("receive", "https://localhost:7207/receive", typeof(string))
        });
    }
    
    [TestMethod]
    public async Task ParsesAsyncSimpleModel()
    {
        // Arrange
        var file = await FileHelper.ReadFileAsync("Models", "AsyncModel.model");

        // Act
        var actual = SpecificationParser.ParseSpecification(file);

        // Assert
        actual.InitialLocation.Name.Should().Be("l0");
        actual.InitialVariables.Should().BeEquivalentTo(new List<Variable>
        {
            new()
            {
                Name = "x",
                Value = "0",
                Type = typeof(int)
            },
            new()
            {
                Name = "y",
                Value = "0",
                Type = typeof(int)
            }
        });
        actual.Switches.Should().BeEquivalentTo(new List<Switch>
        {
            new
            (
                From: new Location("l0"),
                Gate: new Gate(ActionType.Input, "?inX"),
                Guards: new List<Guard>
                {
                    new()
                    {
                        LeftOperand = "x",
                        Operation = Operation.Equals,
                        RightOperand = "0"
                    }
                },
                To: new Location("l1"),
                UpdateMapping: new UpdateMapping
                {
                    AssociatedVariableName = "x",
                    UpdateStatement = new UpdateStatement("x", Operation.Addition, "1")
                }
            ),
            new
            (
                From: new Location("l1"),
                Gate: new Gate(ActionType.Output, "!outX"),
                Guards: new List<Guard>
                {
                    new()
                    {
                        LeftOperand = "x",
                        Operation = Operation.Equals,
                        RightOperand = "1"
                    }
                },
                To: new Location("l2"),
                UpdateMapping: null
            ),
            new
            (
                From: new Location("l2"),
                Gate: new Gate(ActionType.Output, "!outY"),
                Guards: new List<Guard>
                {
                    new()
                    {
                        LeftOperand = "y",
                        Operation = Operation.Equals,
                        RightOperand = "0"
                    }
                },
                To: new Location("l3"),
                UpdateMapping: null
            ),
            // Switches completed by async
            new
            (
                From: new Location("l1"),
                Gate: new Gate(ActionType.Output, "!outY"),
                Guards: new List<Guard>
                {
                    new()
                    {
                        LeftOperand = "y",
                        Operation = Operation.Equals,
                        RightOperand = "0"
                    }
                },
                To: new Location("@l0"),
                UpdateMapping: null
            ),
            new
            (
                From: new Location("@l0"),
                Gate: new Gate(ActionType.Output, "!outX"),
                Guards: new List<Guard>
                {
                    new()
                    {
                        LeftOperand = "x",
                        Operation = Operation.Equals,
                        RightOperand = "1"
                    }
                },
                To: new Location("l3"),
                UpdateMapping: null
            )
        });
    }
    
    [TestMethod]
    public async Task ParsesAsyncMixedModel()
    {
        // Arrange
        var file = await FileHelper.ReadFileAsync("Models", "MixedAsyncModel.model");

        // Act
        var actual = SpecificationParser.ParseSpecification(file);

        // Assert
        actual.InitialLocation.Name.Should().Be("l0");
        actual.InitialVariables.Should().BeEquivalentTo(new List<Variable>
        {
            new()
            {
                Name = "x",
                Value = "0",
                Type = typeof(int)
            },
            new()
            {
                Name = "y",
                Value = "0",
                Type = typeof(int)
            }
        });
        actual.Switches.Should().BeEquivalentTo(new List<Switch>
        {
            new
            (
                From: new Location("l0"),
                Gate: new Gate(ActionType.Input, "?inX"),
                Guards: new List<Guard>
                {
                    new()
                    {
                        LeftOperand = "x",
                        Operation = Operation.Equals,
                        RightOperand = "0"
                    }
                },
                To: new Location("l1"),
                UpdateMapping: new UpdateMapping
                {
                    AssociatedVariableName = "x",
                    UpdateStatement = new UpdateStatement("x", Operation.Addition, "1")
                }
            ),
            new
            (
                From: new Location("l1"),
                Gate: new Gate(ActionType.Output, "!outX"),
                Guards: new List<Guard>
                {
                    new()
                    {
                        LeftOperand = "x",
                        Operation = Operation.Equals,
                        RightOperand = "1"
                    }
                },
                To: new Location("l2"),
                UpdateMapping: null
            ),
            new
            (
                From: new Location("l2"),
                Gate: new Gate(ActionType.Output, "!outY"),
                Guards: new List<Guard>
                {
                    new()
                    {
                        LeftOperand = "y",
                        Operation = Operation.Equals,
                        RightOperand = "0"
                    }
                },
                To: new Location("l3"),
                UpdateMapping: null
            ),
            // Switches completed by async
            new
            (
                From: new Location("l1"),
                Gate: new Gate(ActionType.Output, "!outY"),
                Guards: new List<Guard>
                {
                    new()
                    {
                        LeftOperand = "y",
                        Operation = Operation.Equals,
                        RightOperand = "0"
                    }
                },
                To: new Location("@l0"),
                UpdateMapping: null
            ),
            new
            (
                From: new Location("@l0"),
                Gate: new Gate(ActionType.Output, "!outX"),
                Guards: new List<Guard>
                {
                    new()
                    {
                        LeftOperand = "x",
                        Operation = Operation.Equals,
                        RightOperand = "1"
                    }
                },
                To: new Location("l3"),
                UpdateMapping: null
            ),
            new
            (
                From: new Location("l3"),
                Gate: new Gate(ActionType.Input, "?input"),
                Guards: new List<Guard>(),
                To: new Location("l4"),
                UpdateMapping: null
            ),
            new
            (
                From: new Location("l4"),
                Gate: new Gate(ActionType.Output, "!output"),
                Guards: new List<Guard>(),
                To: new Location("l5"),
                UpdateMapping: null
            ),
            new
            (
                From: new Location("l5"),
                Gate: new Gate(ActionType.Input, "?input2"),
                Guards: new List<Guard>(),
                To: new Location("l6"),
                UpdateMapping: null
            ),
            new
            (
                From: new Location("l6"),
                Gate: new Gate(ActionType.Output, "!outX"),
                Guards: new List<Guard>(),
                To: new Location("l7"),
                UpdateMapping: null
            ),
            new
            (
                From: new Location("l7"),
                Gate: new Gate(ActionType.Output, "!outY"),
                Guards: new List<Guard>(),
                To: new Location("l8"),
                UpdateMapping: null
            ),
            new
            (
                From: new Location("l6"),
                Gate: new Gate(ActionType.Output, "!outY"),
                Guards: new List<Guard>(),
                To: new Location("@l1"),
                UpdateMapping: null
            ),
            new
            (
                From: new Location("@l1"),
                Gate: new Gate(ActionType.Output, "!outX"),
                Guards: new List<Guard>(),
                To: new Location("l8"),
                UpdateMapping: null
            )
        });
    }
    
    [TestMethod]
    public async Task ParsesLargeAsyncModel3()
    {
        // Arrange
        var file = await FileHelper.ReadFileAsync("Models", "LargeAsyncModel3.model");

        // Act
        var actual = SpecificationParser.ParseSpecification(file);

        // Assert
        actual.Switches.Should().HaveCount(19);
    }
    
    [TestMethod]
    public async Task ParsesLargeAsyncModel4()
    {
        // Arrange
        var file = await FileHelper.ReadFileAsync("Models", "LargeAsyncModel4.model");

        // Act
        var actual = SpecificationParser.ParseSpecification(file);

        // Assert
        actual.Switches.Should().HaveCount(97);
    }
    
    [TestMethod]
    public async Task ParsesLargeAsyncModel5()
    {
        // Arrange
        var file = await FileHelper.ReadFileAsync("Models", "LargeAsyncModel5.model");

        // Act
        var actual = SpecificationParser.ParseSpecification(file);

        // Assert
        actual.Switches.Should().HaveCount(601);
    }
}