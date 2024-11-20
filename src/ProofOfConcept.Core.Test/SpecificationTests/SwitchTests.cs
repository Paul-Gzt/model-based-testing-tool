using FluentAssertions;
using ProofOfConcept.Core.Specifications;
using ProofOfConcept.Infrastructure.Utility;

namespace ProofOfConcept.Core.Test.SpecificationTests;

[TestClass]
public class SwitchTests
{
    [TestMethod]
    public async Task Switch_ValidSwitch_Switches()
    {
        // Arrange
        var specification = await ModelReader.ReadFromFileAsync("Models", "SegTest1.model");
        var input = new Gate(ActionType.Input, "?inX");
        
        // Act
        specification.CurrentState.InstantiatedLocations.Should().BeEquivalentTo(new List<Location> { new("l0") });
        var result = specification.Switch(input);
        
        // Assert
        specification.CurrentState.InstantiatedLocations.Should().BeEquivalentTo(new List<Location> { new("l1") });
        result.Item1.Should().BeTrue();
    }

    [DataRow(ActionType.Output, "!outX")]
    [DataRow(ActionType.Output, "!inX")]
    [DataRow(ActionType.Output, "?inX")]
    [DataRow(ActionType.Output, "SomeVeryWeirdValue")]
    [DataTestMethod]
    public async Task Switch_InvalidSwitch_DoesNothing(ActionType actionType, string gateLabel)
    {
        // Arrange
        var specification = await ModelReader.ReadFromFileAsync("Models", "SegTest1.model");
        var input = new Gate(actionType, gateLabel);

        // Act
        specification.CurrentState.InstantiatedLocations.Should().BeEquivalentTo(new List<Location> { new("l0") });
        var result = specification.Switch(input);

        // Assert
        specification.CurrentState.InstantiatedLocations.Should().BeEquivalentTo(new List<Location> { new("l0") });
        result.Item1.Should().BeFalse();
    }
    
    [TestMethod]
    public async Task Switch_TwoNonDeterministicChoices_Switches()
    {
        // Arrange
        var specification = await ModelReader.ReadFromFileAsync("Models", "NonDeterministicChoices.model");
        var input = new Gate(ActionType.Input, "?input");
        
        // Act
        specification.CurrentState.InstantiatedLocations.Should().BeEquivalentTo(new List<Location> { new("l0") });
        var result = specification.Switch(input);
        
        specification.CurrentState.InstantiatedLocations.Should().BeEquivalentTo(new List<Location> { new("l1")});
        result.Item1.Should().BeTrue();

        var secondInput = new Gate(ActionType.Output, "!output1");
        var secondResult = specification.Switch(secondInput);
        
        // Assert
        specification.CurrentState.InstantiatedLocations.Should().BeEquivalentTo(new List<Location> { new("l2"), new("l3") });
        secondResult.Item1.Should().BeTrue();
    }
    
    [TestMethod]
    public async Task Switch_TwoNonDeterministicChoices_Continuation_Switches()
    {
        // Arrange
        var specification = await ModelReader.ReadFromFileAsync("Models", "NonDeterministicChoices2.model");
        var input = new Gate(ActionType.Input, "?input");
        
        // Act
        specification.CurrentState.InstantiatedLocations.Should().BeEquivalentTo(new List<Location> { new("l0") });
        specification.Switch(input);

        var secondInput = new Gate(ActionType.Output, "!output1");
        specification.Switch(secondInput);

        var thirdInput = new Gate(ActionType.Input, "?input2");
        var result = specification.Switch(thirdInput);
        
        // Assert
        specification.CurrentState.InstantiatedLocations.Should().BeEquivalentTo(new List<Location> { new("l4") });
        result.Item1.Should().BeTrue();
        
    }
    
    [TestMethod]
    public async Task Switch_TwoDeterministicChoices_Switches()
    {
        // Arrange
        var specification = await ModelReader.ReadFromFileAsync("Models", "DeterministicChoices.model");
        var input = new Gate(ActionType.Input, "?input");
        
        // Act
        specification.CurrentState.InstantiatedLocations.Should().BeEquivalentTo(new List<Location> { new("l0") });
        var result = specification.Switch(input);
        
        specification.CurrentState.InstantiatedLocations.Should().BeEquivalentTo(new List<Location> { new("l1")});
        result.Item1.Should().BeTrue();

        var secondInput = new Gate(ActionType.Output, "!output2");
        var secondResult = specification.Switch(secondInput);
        
        // Assert
        specification.CurrentState.InstantiatedLocations.Should().BeEquivalentTo(new List<Location> { new("l3") });
        secondResult.Item1.Should().BeTrue();
    }
}