namespace ProofOfConcept.Core.Specifications;

public readonly record struct Gate(ActionType ActionType, string Label)
{
    public static ActionType DetermineActionType(string input)
    {
        if (input.StartsWith("?")) return ActionType.Input;
        if (input.StartsWith("!")) return ActionType.Output;
        if (input.StartsWith("0")) return ActionType.Quiescence;
        if (input.StartsWith("T")) return ActionType.Tau;

        return ActionType.Unknown;
    }
}