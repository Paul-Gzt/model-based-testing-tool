using ProofOfConcept.Core.Specifications;

namespace ProofOfConcept.Infrastructure.Microservices.Output;

public class OutputResponseAdapter : OutputAdapterBase
{
    private List<Trace> _traces;

    protected OutputResponseAdapter()
    {
        _traces = new List<Trace>();
    }
    
    public override void ReceiveResponse(Trace? trace)
    {
        if (trace is null || trace.Value.Result.ActionType == ActionType.Unknown) return;
        
        _traces.Add(trace.Value);
    }

    protected override List<Trace> Observe()
    {
        var result = new List<Trace>();
        result.AddRange(_traces);

        _traces = new List<Trace>();
        
        return result;
    }
}