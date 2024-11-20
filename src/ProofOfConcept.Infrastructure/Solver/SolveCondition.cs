using Condition = ProofOfConcept.Core.Solver.Condition;

namespace ProofOfConcept.Infrastructure.Solver;

public readonly record struct SolveCondition
{
    public SolveVariable Variable { get; }

    public ArithmeticOperation ArithmeticOperation { get; }

    public BooleanOperation BooleanOperation { get; }

    public SolveValue Value { get; }
    
    public bool IsStringOperation => Variable.Type == Type.String;

    public bool IsArithmeticOperation => ArithmeticOperation != ArithmeticOperation.Unknown;

    public bool IsBooleanOperation => BooleanOperation != BooleanOperation.Unknown;

    public SolveCondition(SolveVariable variable, ArithmeticOperation arithmeticOperation, SolveValue value)
    {
        Variable = variable;
        ArithmeticOperation = arithmeticOperation;
        BooleanOperation = BooleanOperation.Unknown;
        Value = value;
    }

    public SolveCondition(SolveVariable variable, BooleanOperation booleanOperation, SolveValue value)
    {
        Variable = variable;
        ArithmeticOperation = ArithmeticOperation.Unknown;
        BooleanOperation = booleanOperation;
        Value = value;
    }

    public static SolveCondition CreateFrom(Condition condition)
    {
        return new SolveCondition(SolveVariable.CreateFrom(condition.Variable), ArithmeticOperation.Equals, SolveValue.CreateFrom(condition.Value));
    }
}
