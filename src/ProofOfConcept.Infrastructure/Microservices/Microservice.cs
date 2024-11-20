using ProofOfConcept.Core.Specifications;
using ProofOfConcept.Core.Testing;
using ProofOfConcept.Infrastructure.Microservices.Input;
using ProofOfConcept.Infrastructure.Microservices.Output;

namespace ProofOfConcept.Infrastructure.Microservices;

public class Microservice : ISystemUnderTest
{
    private readonly IInputAdapter _inputAdapters;
    private readonly IOutputAdapter _outputAdapters;

    private List<Trace> _cachedTraces;
    
    public Microservice(AdapterChain<IInputAdapter> inputAdapterChain,
        AdapterChain<IOutputAdapter> outputAdapterChain)
    {
        _inputAdapters = inputAdapterChain.Chain;
        _outputAdapters = outputAdapterChain.Chain;
        _cachedTraces = new List<Trace>();
    }
    
    public async Task SendInput(string label, List<Parameter> parameters)
    {
        await _inputAdapters.PerformActionAsync(label, parameters);
    }

    public Trace ReceiveOutput()
    {
        var observedTraces = _outputAdapters.ObserveOutput();

        if (!observedTraces.Any()) return GetObservedTraceOrCache(new Trace());

        foreach (var trace in observedTraces)
        {
            Console.WriteLine("Observed the following output from the SUT: {0}", trace);
        }

        var traceToReturn = GetObservedTraceOrCache(observedTraces.First());

        if (observedTraces.Count == 1) return traceToReturn;
        
        _cachedTraces = _cachedTraces
            .Union(observedTraces.Skip(1))
            .ToList();

        return traceToReturn;
    }

    private Trace GetObservedTraceOrCache(Trace observedTrace)
    {
        if (!_cachedTraces.Any()) return observedTrace;
        
        var firstObservedTrace = _cachedTraces.First();
        _cachedTraces = _cachedTraces.Skip(1).ToList();

        if (observedTrace.Result.ActionType != ActionType.Unknown)
        {
            _cachedTraces.Add(observedTrace);
        }
        
        return firstObservedTrace;
    }
}