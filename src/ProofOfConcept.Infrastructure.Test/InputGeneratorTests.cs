using FluentAssertions;
using ProofOfConcept.Infrastructure.Generation;
using ProofOfConcept.Infrastructure.Solver;
using Type = ProofOfConcept.Infrastructure.Solver.Type;

namespace ProofOfConcept.Infrastructure.Test;

[TestClass]
public class InputGeneratorTests
{
    [TestMethod]
    public void GenInput_LessThan()
    {
        // Arrange
        var sut = new InputGenerator();
        // p < 10
        var condition = new SolveCondition(new SolveVariable("p", Type.Int), ArithmeticOperation.LessThan, new SolveValue("10", Type.Int));
        
        // Act
        var actual = sut.GenerateValue(condition);
        
        // Assert
        actual.Should().Be("9");
    }

    [TestMethod]
    public void GenInput_Equals()
    {
        // Arrange
        var sut = new InputGenerator();
        // p < 10
        var condition = new SolveCondition(new SolveVariable("p", Type.Int), ArithmeticOperation.Equals,
            new SolveValue("10", Type.Int));

        // Act
        var actual = sut.GenerateValue(condition);

        // Assert
        actual.Should().Be("10");
    }

    [TestMethod]
    public void GenInput_Equals_String()
    {
        // Arrange
        var sut = new InputGenerator();
        // p < 10
        var condition = new SolveCondition(new SolveVariable("p", Type.String), ArithmeticOperation.Equals,
            new SolveValue("WeatherForecast", Type.String));

        // Act
        var actual = sut.GenerateValue(condition);

        // Assert
        actual.Should().Be("WeatherForecast");
    }
}