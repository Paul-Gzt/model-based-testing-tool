namespace ProofOfConcept.Core.Assertion;

public readonly record struct AssertionResult(bool Success, string Reason);