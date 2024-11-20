using ProofOfConcept.Core.Solver;

namespace ProofOfConcept.Infrastructure.Solver;

// TODO: Values can be variables? x > y
public readonly record struct SolveValue(string Value, Type Type)
{
    // TODO: Type
    public static SolveValue CreateFrom(Value value)
    {
        return new SolveValue(value.ValueContent, Type.Int);
    }
}