using ProofOfConcept.Core.Solver;

namespace ProofOfConcept.Core.Specifications;

// TODO: Use of actual variables: x > 5, x > y
public readonly record struct Guard(string LeftOperand, Operation Operation, string RightOperand);