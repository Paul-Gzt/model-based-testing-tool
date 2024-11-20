using ProofOfConcept.Core.Specifications;

namespace ProofOfConcept.Infrastructure.Sts.Subparts;

public record Switch(string From, string Label, string To, string? WhereClause, string? UpdateMapping, bool IsAsync)
{
    public static ProofOfConcept.Core.Specifications.Switch CreateFrom(Switch input)
    {
        return new ProofOfConcept.Core.Specifications.Switch(
            From: new Location(input.From),
            Gate: new Gate(Gate.DetermineActionType(input.Label), RemoveAsyncModifiers(RemoveParameters(input.Label))),
            Guards: GuardParser.ParseGuard(input.WhereClause),
            UpdateMapping: UpdateMappingParser.ParseUpdateMapping(input.UpdateMapping),
            To: new Location(input.To)
        );
    }
    
    /// <summary>
    /// Labels can have parameters, such as ?http_get(endpoint)
    /// In this case, this method returns ?http_get
    /// </summary>
    /// <param name="guardLabel"></param>
    /// <returns></returns>
    private static string RemoveParameters(string guardLabel)
    {
        var split = guardLabel.Split("(");
        return split.First();
    }

    /// <summary>
    /// Labels can have async modifiers, such as !@http_ok
    /// In this case, this method returns !http_ok
    /// </summary>
    /// <param name="guardLabel"></param>
    /// <returns></returns>
    private static string RemoveAsyncModifiers(string guardLabel)
    {
        if (!guardLabel.StartsWith("!@")) return guardLabel;
        return guardLabel.Replace("!@", "!");
    }
}