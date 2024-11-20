using ProofOfConcept.Core.Solver;

namespace ProofOfConcept.Core.Specifications;

public readonly record struct UpdateStatement(string LeftHandValue, Operation Operation, string RightHandValue);

public readonly record struct UpdateMapping(string AssociatedVariableName, UpdateStatement UpdateStatement);