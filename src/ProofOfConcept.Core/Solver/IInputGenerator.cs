using ProofOfConcept.Core.Specifications;
using ProofOfConcept.Core.Testing;

namespace ProofOfConcept.Core.Solver;

public interface IInputGenerator
{
    List<Parameter> GenerateValue(List<Guard> guards, List<Variable> variables);
}