using ProofOfConcept.Core.Specifications;

namespace ProofOfConcept.Infrastructure.Microservices.Output;

public interface IOutputAdapter : IAdapter
{ 
    List<Trace> ObserveOutput();
}