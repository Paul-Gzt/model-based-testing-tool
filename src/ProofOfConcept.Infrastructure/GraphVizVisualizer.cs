using System.Text;
using ProofOfConcept.Core.Solver;
using ProofOfConcept.Core.Specifications;

namespace ProofOfConcept.Infrastructure;

public static class GraphVizVisualizer
{
    public static Stream Visualize(Specification specification)
    {
        var fileLines = new List<string>
        {
            "digraph G {",
            @$"init [xlabel=""{VisualizeInitializer(specification)}"", shape=point]",
            @$"""init"" -> ""{specification.InitialLocation.Name}"""
        };
    
        foreach (var transition in specification.Switches)
        {
            fileLines.Add(@$"""{transition.From.Name}"" -> ""{transition.To.Name}"" [""label"" = ""{transition.Gate.Label}\n {VisualizeGuard(transition.Guards)}\n {VisualizeUpdateMapping(transition.UpdateMapping)}""]");
        }
            
        fileLines.Add("}");
    
        var fileContent = string.Join(Environment.NewLine, fileLines.Select(a => String.Join("\n", a)));            
        return new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
    }

    private static string VisualizeGuard(List<Guard> guards)
    {
        if (!guards.Any()) return "";
        var stringBuilder = new StringBuilder();

        for (var index = 0; index < guards.Count; index++)
        {
            var guard = guards[index];
            var operation = VisualizeOperation(guard.Operation);
            stringBuilder.Append($"{guard.LeftOperand} {operation} {guard.RightOperand.Replace("\"", string.Empty)}");

            if (index != guards.Count - 1)
            {
                stringBuilder.Append(" &&\n");
            }
        }

        return stringBuilder.ToString();
    }

    private static string VisualizeOperation(Operation operation)
    {
        return operation switch
        {
            Operation.Equals => "==",
            Operation.Addition => "+",
            Operation.LessThan => "<",
            _ => "???"
        };
    }

    private static string VisualizeUpdateMapping(UpdateMapping? updateMapping)
    {
        if (updateMapping is null) return "";

        return $"{updateMapping.Value.AssociatedVariableName} = " +
               $"{updateMapping.Value.UpdateStatement.LeftHandValue} {VisualizeOperation(updateMapping.Value.UpdateStatement.Operation)} {updateMapping.Value.UpdateStatement.RightHandValue}";
    }

    private static string VisualizeInitializer(Specification specification)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("{");

        foreach (var variableDeclaration in specification.InitialVariables)
        {
            stringBuilder.AppendLine($"{variableDeclaration.Name}: {variableDeclaration.Value}");
        }

        stringBuilder.AppendLine("}");

        return stringBuilder.ToString();
    }
    
}