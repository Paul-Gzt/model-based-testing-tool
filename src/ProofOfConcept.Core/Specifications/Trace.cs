namespace ProofOfConcept.Core.Specifications;

public readonly record struct Trace(Gate Result, string Value)
{
    public static Trace Empty => new Trace(new Gate(ActionType.Unknown, string.Empty), string.Empty);
}