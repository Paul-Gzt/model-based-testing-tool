using FluentAssertions;
using ProofOfConcept.Core.Solver;
using ProofOfConcept.Infrastructure.Solver;
using Type = ProofOfConcept.Infrastructure.Solver.Type;

namespace ProofOfConcept.Infrastructure.Test;

[TestClass]
public class Z3SolverTests
{
    private Solver.ConstraintSolver _sut = null!;
    
    [TestInitialize]
    public void TestInitialize()
    {
        _sut = new Solver.ConstraintSolver();
    }

    // TODO: Z3 Context for mocking
    // TODO: Unsatisfiable tests for ints, we need to create larger equations for this: (p = 10 AND p < 8)
    [DataRow("x", ArithmeticOperation.Equals, "-2147483648")]
    [DataRow("x", ArithmeticOperation.Equals, "2147483648")]
    [DataRow("x", ArithmeticOperation.Equals, "10")]
    [DataRow("y", ArithmeticOperation.GreaterThan, "12")]
    [DataRow("bla", ArithmeticOperation.GreaterOrEqualsThan, "300")]
    [DataRow("p", ArithmeticOperation.LessThan, "-10")]
    [DataRow("longerVariableName", ArithmeticOperation.LessOrEqualsThan, "0")]
    [DataTestMethod]
    public void SimpleSatisfiableIntTests(string variableName, ArithmeticOperation arithmeticOperation, string valueAsInt)
    {
        // Arrange
        var condition = new SolveCondition(
            new SolveVariable(variableName, Type.Int),
            arithmeticOperation,
            new SolveValue(valueAsInt, Type.Int)
        );
        
        // Act
        var actual = _sut.Solve(condition);
        
        // Assert
        actual.Should().Be(SolveResult.Satisfiable);
    }
    
    [DataRow("-2147483648", ArithmeticOperation.Equals, "-2147483648")]
    [DataRow("2147483648", ArithmeticOperation.Equals, "2147483648")]
    [DataRow("SomeValue", ArithmeticOperation.Equals, "SomeValue")]
    [DataRow("aaaaaaaaaaa", ArithmeticOperation.Equals, "aaaaaaaaaaa")]
    [DataTestMethod]
    public void SimpleSatisfiableStringTests(string variableName, ArithmeticOperation arithmeticOperation, string valueAsInt)
    {
        // Arrange
        var condition = new SolveCondition(
            new SolveVariable(variableName, Type.String),
            arithmeticOperation,
            new SolveValue(valueAsInt, Type.String)
        );

        // Act
        var actual = _sut.Solve(condition);

        // Assert
        actual.Should().Be(SolveResult.Satisfiable);
    }

    [DataRow("-2147483648", ArithmeticOperation.Equals, "-1")]
    [DataRow("2147483648", ArithmeticOperation.Equals, "1")]
    [DataRow("SomeValue", ArithmeticOperation.Equals, "SomeDifferentValue")]
    [DataRow("aaaaaaaaaaa", ArithmeticOperation.Equals, "bbbbbbbbb")]
    [DataTestMethod]
    public void SimpleUnsatisfiableStringTests(string variableName, ArithmeticOperation arithmeticOperation,
        string valueAsInt)
    {
        // Arrange
        var condition = new SolveCondition(
            new SolveVariable(variableName, Type.String),
            arithmeticOperation,
            new SolveValue(valueAsInt, Type.String)
        );

        // Act
        var actual = _sut.Solve(condition);

        // Assert
        actual.Should().Be(SolveResult.Unsatisfiable);
    }

    [DataRow("true", BooleanOperation.Equals, "true")]
    [DataRow("true", BooleanOperation.NotEquals, "false")]
    [DataRow("true", BooleanOperation.And, "true")]
    [DataRow("false", BooleanOperation.Or, "true")]
    [DataTestMethod]
    public void SimpleSatisfiableBoolTests(string variableName, BooleanOperation booleanOperation, string valueAsBoolean)
    {
        // Arrange
        var condition = new SolveCondition(
            new SolveVariable(variableName, Type.Boolean),
            booleanOperation,
            new SolveValue(valueAsBoolean, Type.Boolean)
        );

        // Act
        var actual = _sut.Solve(condition);

        // Assert
        actual.Should().Be(SolveResult.Satisfiable);
    }
    
    [DataRow("true", BooleanOperation.And, "false")]
    [DataTestMethod]
    public void SimpleUnsatisfiableBoolTests(string variableName, BooleanOperation booleanOperation, string valueAsBoolean)
    {
        // Arrange
        var condition = new SolveCondition(
            new SolveVariable(variableName, Type.Boolean),
            booleanOperation,
            new SolveValue(valueAsBoolean, Type.Boolean)
        );

        // Act
        var actual = _sut.Solve(condition);

        // Assert
        actual.Should().Be(SolveResult.Unsatisfiable);
    }
}