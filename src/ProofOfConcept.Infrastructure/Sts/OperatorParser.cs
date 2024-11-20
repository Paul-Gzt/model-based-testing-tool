using ProofOfConcept.Core.Solver;

namespace ProofOfConcept.Infrastructure.Sts;

public static class OperatorParser
{
    public static Operation ParseOperator(string operatorAsString)
    {
        return operatorAsString switch
        {
            "==" => Operation.Equals,
            "<" => Operation.LessThan,
            "+" => Operation.Addition,
            _ => Operation.Unknown
        };
    }
}