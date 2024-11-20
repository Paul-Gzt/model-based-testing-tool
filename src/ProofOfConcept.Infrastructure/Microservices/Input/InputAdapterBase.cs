using ProofOfConcept.Core.Specifications;
using ProofOfConcept.Core.Testing;
using ProofOfConcept.Infrastructure.Microservices.Output;

namespace ProofOfConcept.Infrastructure.Microservices.Input;

public abstract class InputAdapterBase : IInputAdapter
{
    private InputAdapterBase? _nextAdapter;
    private OutputAdapterBase? _outputAdapter;

    protected InputAdapterBase(List<string> supportedLabels)
    {
        SupportedLabels = supportedLabels;
    }

    private List<string> SupportedLabels { get; }

    public async Task PerformActionAsync(string label, List<Parameter> parameters)
    {
        if (SupportedLabels.Contains(label))
        {
            var trace = await DoAction(label, parameters);

            if (_outputAdapter is null) return;
            _outputAdapter.ReceiveResponse(trace);
        }
        if (_nextAdapter is null) return;
        
        await _nextAdapter.PerformActionAsync(label, parameters);
    }

    protected abstract Task<Trace?> DoAction(string label, List<Parameter> parameters);

    public void SetNext(IAdapter adapter)
    {
        if (adapter is InputAdapterBase inputAdapterBase)
        {
            _nextAdapter = inputAdapterBase;
        }
    }

    public InputAdapterBase SetOutputAdapter(OutputAdapterBase adapter)
    {
        _outputAdapter = adapter;
        return this;
    }
}