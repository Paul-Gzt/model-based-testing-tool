namespace ProofOfConcept.Core.Solver;

public readonly record struct Condition(Variable Variable, Operation Operation, Value Value);