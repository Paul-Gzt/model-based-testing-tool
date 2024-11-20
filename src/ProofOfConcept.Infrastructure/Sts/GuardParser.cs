using ProofOfConcept.Core.Specifications;

namespace ProofOfConcept.Infrastructure.Sts;

public static class GuardParser
{
    public static List<Guard> ParseGuard(string? guardStatement)
    {
        if (string.IsNullOrEmpty(guardStatement)) return new List<Guard>();
        
        return guardStatement
            .Split(" && ")
            .Select(guard => guard.Split(" "))
            .Select(guardStatements => 
                new Guard(guardStatements[0], 
                    OperatorParser.ParseOperator(guardStatements[1]), 
                    guardStatements[2]))
            .ToList();
    }
}