using System.Text;
using ProofOfConcept.Core.Specifications;

namespace ProofOfConcept.Core.Testing.Reporting;

// TODO: We can do value comparison here
// TODO: We can show if this trace was correct, wrong by its label, or wrong by its data
public readonly record struct OutputTrace(string Label, Trace Output, List<Gate> Expected) : ITrace
{
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine($"Expected: '{Label}'");
        stringBuilder.AppendLine($"Observed: '{Output.Result.Label}'");
        stringBuilder.AppendLine($"Observed: '{Output.Value}'");

        if (!Expected.Any()) return stringBuilder.ToString();
        
        stringBuilder.AppendLine($"Actions within expectations:");

        foreach (var gate in Expected)
        {
            stringBuilder.AppendLine($"!{gate.Label}");
        }

        return stringBuilder.ToString();
    }
}