using FluentAssertions;
using ProofOfConcept.Core.Specifications;

namespace ProofOfConcept.Core.Test;

[TestClass]
public class GuardTests
{

    // [DataRow(int.MaxValue, Operator.Equals, int.MaxValue, true)]
    // [DataRow(int.MinValue, Operator.Equals, int.MinValue, true)]
    // [DataRow(1, Operator.Equals, 1, true)]
    // [DataRow(0, Operator.Equals, 0, true)]
    // [DataTestMethod]
    // public void EqualsTest(int leftOperandValue, Operator @operator, int rightOperandValue, bool expectedValue)
    // {
    //     // Arrange
    //     var guard = new Guard
    //     {
    //         LeftOperand = new(leftOperandValue),
    //         Operator = @operator,
    //         RightOperand = new(rightOperandValue)
    //     };
    //
    //     // Act
    //     var actual = guard.Value;
    //
    //     // Assert
    //     actual.Should().Be(expectedValue);
    // }
}