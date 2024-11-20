using ProofOfConcept.Core.Solver;

namespace ProofOfConcept.Infrastructure.Solver;

public readonly record struct SolveVariable(string Name, Type Type)
{
    // TODO: Add Type in variable
    public static SolveVariable CreateFrom(Variable variable)
    {
        return new SolveVariable(variable.Name, Type.Int);
    }
}