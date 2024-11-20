using ProofOfConcept.Core.Solver;
using ProofOfConcept.Core.Specifications;
using ProofOfConcept.Core.Testing;
using ProofOfConcept.Infrastructure.Solver;
using Type = ProofOfConcept.Infrastructure.Solver.Type;

namespace ProofOfConcept.Infrastructure.Generation;

public class InputGenerator : IInputGenerator
{
    // TODO: What if there is no guard? We might still need to put a variable in
    // E.g: inX(p) where null => generate any value
    public List<Parameter> GenerateValue(List<Guard> guards, List<Variable> variables)
    {
        if (!guards.Any()) return new List<Parameter>();

        var result = new List<Parameter>();
        
        foreach (var guard in guards)
        {
            var solveValue = GetSolveValue(guard, variables);

            // TODO: Support non-string variables
            var solveCondition = new SolveCondition(
                new SolveVariable(guard.LeftOperand, Type.String),
                ArithmeticOperation.Equals,
                new SolveValue(solveValue, Type.String));

            var foundValue = GenerateValue(solveCondition);
            
            result.Add(new Parameter(guard.LeftOperand, foundValue));
        }

        return result;
    }

    private static string GetSolveValue(Guard guard, List<Variable> variables)
    {
        var isStringValue = guard.RightOperand.Contains('"');
        var isTemplateFile = guard.RightOperand.StartsWith(Constants.TemplatePrefix);

        if (isStringValue || isTemplateFile) return guard.RightOperand;

        return GetVariable(variables, guard.RightOperand);
    }

    private static string GetVariable(List<Variable> variables, string valueRightOperand)
    {
        var found = variables.FirstOrDefault(variable => variable.Name == valueRightOperand);

        return found.Value ?? "";
    }

    public string GenerateValue(SolveCondition condition)
    {
        if (condition.IsStringOperation)
        {
            var stringSolver = new StringSolver();
            
            return stringSolver.TryAndFindValue(condition);
        }
        
        var solver = new ConstraintSolver();

        var result = solver.TryAndFindValue(condition);

        return result.Consts.First().Value.ToString();
    }
}