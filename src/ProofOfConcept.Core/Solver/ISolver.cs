namespace ProofOfConcept.Core.Solver;

public interface ISolver
{
    SolveResult Solve(Condition condition);
}