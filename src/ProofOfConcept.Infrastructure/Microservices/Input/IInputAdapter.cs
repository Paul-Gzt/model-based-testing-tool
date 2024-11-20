using ProofOfConcept.Core.Testing;
using ProofOfConcept.Infrastructure.Microservices.Output;

namespace ProofOfConcept.Infrastructure.Microservices.Input;

public interface IInputAdapter : IAdapter
{
    Task PerformActionAsync(string label, List<Parameter> parameters);

    InputAdapterBase SetOutputAdapter(OutputAdapterBase adapter);
}