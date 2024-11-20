using ProofOfConcept.Core.Specifications;

namespace ProofOfConcept.Infrastructure.Microservices.Output;

public abstract class OutputAdapterBase : IOutputAdapter
{
    private OutputAdapterBase? _nextAdapter;

    public List<Trace> ObserveOutput()
    {
        var traces = Observe();

        if (_nextAdapter is null) return traces;

        traces.AddRange(_nextAdapter.ObserveOutput());

        return traces;
    }

    public abstract void ReceiveResponse(Trace? trace);

    protected abstract List<Trace> Observe();
    
    public void SetNext(IAdapter adapter)
    {
        if (adapter is OutputAdapterBase outputAdapterBase)
        {
            _nextAdapter = outputAdapterBase;   
        }
    }
}