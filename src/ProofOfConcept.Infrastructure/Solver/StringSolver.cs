namespace ProofOfConcept.Infrastructure.Solver;

public class StringSolver
{
    public string TryAndFindValue(SolveCondition condition)
    {
        return condition.ArithmeticOperation switch
        {
            ArithmeticOperation.Equals => condition.Value.Value,
            _ => "UNKNOWN"
        };
    }
}