using System.Text;

namespace ProofOfConcept.Core.Testing.Reporting;

public readonly record struct InputTrace(string Label, List<Parameter> Parameters) : ITrace
{
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append($"{Label}");
        stringBuilder.Append("(");
        for (var index = 0; index < Parameters.Count; index++)
        {
            var parameter = Parameters[index];
            stringBuilder.Append($"{parameter.Name}");

            if (index != Parameters.Count -1)
            {
                stringBuilder.Append(", ");
            }
        }

        stringBuilder.Append(")");
        stringBuilder.AppendLine("With data:");
        foreach (var parameter in Parameters)
        {
            stringBuilder.AppendLine($"{parameter.Name} - {parameter.Value}");
        }

        return stringBuilder.ToString();
    }
}