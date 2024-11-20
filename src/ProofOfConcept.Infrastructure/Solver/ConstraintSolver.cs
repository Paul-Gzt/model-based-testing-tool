using Microsoft.Z3;
using ProofOfConcept.Core.Solver;

namespace ProofOfConcept.Infrastructure.Solver;

public class ConstraintSolver : ISolver
{
    public Model TryAndFindValue(SolveCondition condition)
    {
        using var context = new Context();

        // Create formula to solve
        var formula = CreateFormula(context, condition);

        // Solve formula
        var solver = context.MkSolver();
        solver.Assert(formula);
        var solveResult = solver.Check();

        return solver.Model;
    }
    
    public SolveResult Solve(Condition condition)
    {
        var solveCondition = SolveCondition.CreateFrom(condition);
        return Solve(solveCondition);
    }

    public SolveResult Solve(SolveCondition condition)
    {
        using var context = new Context();

        // Create formula to solve
        var formula = CreateFormula(context, condition);

        // Solve formula
        var solver = context.MkSolver();
        solver.Assert(formula);
        var solveResult = solver.Check();

        return MapToSolveResult(solveResult);
    }
    
    private static BoolExpr CreateFormula(Context context, SolveCondition condition)
    {
        if (condition.IsArithmeticOperation)
        {
            return CreateArithmeticFormula(context, condition);
        }

        if (condition.IsBooleanOperation)
        {
            return CreateBooleanFormula(context, condition);
        }

        throw new SolverException("Operation not supported");
    }

    private static BoolExpr CreateBooleanFormula(Context context, SolveCondition condition)
    {
        BoolExpr leftHandVariable = (BoolExpr) CreateVariable(context, condition.Variable);
        BoolExpr rightHandVariable = (BoolExpr) CreateValue(context, condition.Value);
        
        return condition.BooleanOperation switch
        {
            BooleanOperation.Equals => context.MkEq(leftHandVariable, rightHandVariable),
            BooleanOperation.And => context.MkAnd(leftHandVariable, rightHandVariable),
            BooleanOperation.Or => context.MkOr(leftHandVariable, rightHandVariable),
            BooleanOperation.NotEquals => context.MkNot(context.MkEq(leftHandVariable, rightHandVariable)),
            BooleanOperation.Unknown => throw new ArgumentException(
                $"Cannot create formula for {ArithmeticOperation.Unknown.ToString()}"),
            _ => throw new SolverException($"Unknown operation {condition.BooleanOperation}")
        };
    }

    private static BoolExpr CreateArithmeticFormula(Context context, SolveCondition condition)
    {
        var leftHandVariable = CreateVariable(context, condition.Variable);
        var rightHandVariable = CreateValue(context, condition.Value);
        
        if (leftHandVariable is IntExpr leftHandInt && rightHandVariable is IntExpr rightHandInt)
        {
            return condition.ArithmeticOperation switch
            {
                ArithmeticOperation.Equals => context.MkEq(leftHandInt, rightHandInt),
                ArithmeticOperation.GreaterThan => context.MkGt(leftHandInt, rightHandInt),
                ArithmeticOperation.GreaterOrEqualsThan => context.MkGe(leftHandInt, rightHandInt),
                ArithmeticOperation.LessThan => context.MkLt(leftHandInt, rightHandInt),
                ArithmeticOperation.LessOrEqualsThan => context.MkLe(leftHandInt, rightHandInt),
                ArithmeticOperation.Unknown => throw new ArgumentException($"Cannot create formula for {ArithmeticOperation.Unknown.ToString()}"),
                _ => throw new SolverException($"Unknown operation {condition.ArithmeticOperation}")
            };
        }

        if (leftHandVariable is SeqExpr leftHandString && rightHandVariable is SeqExpr rightHandString)
        {
            return condition.ArithmeticOperation switch
            {
                ArithmeticOperation.Equals => context.MkEq(leftHandString, rightHandString),
                ArithmeticOperation.Unknown => throw new ArgumentException(
                    $"Cannot create formula for {ArithmeticOperation.Unknown.ToString()}"),
                _ => throw new SolverException($"Unknown operation {condition.ArithmeticOperation}")
            };
        }

        throw new ArgumentException("Type not supported");
    }

    private static Expr CreateVariable(Context context, SolveVariable variable)
    {
        return variable.Type switch
        {
            Type.Int => context.MkIntConst(variable.Name),
            Type.String => context.MkString(variable.Name),
            Type.Boolean => context.MkBoolConst(variable.Name),
            _ => throw new ArgumentOutOfRangeException(nameof(variable), variable, null)
        };
    }

    private static Expr CreateValue(Context context, SolveValue value)
    {
        return value.Type switch
        {
            Type.Int => context.MkInt(value.Value),
            Type.String => context.MkString(value.Value), // TODO: We don't support strings stored in variables
            Type.Boolean => context.MkBool(ParseBool(value.Value)),
            Type.Unknown => throw new ArgumentException(
                $"Cannot create formula for {ArithmeticOperation.Unknown.ToString()}"),
            _ => throw new SolverException($"{value.Type} is not supported")
        };
    }
    
    private static SolveResult MapToSolveResult(Status status)
    {
        return status switch
        {
            Status.SATISFIABLE => SolveResult.Satisfiable,
            Status.UNSATISFIABLE => SolveResult.Unsatisfiable,
            Status.UNKNOWN => SolveResult.Unknown,
            _ => SolveResult.Unknown
        };
    }

    private static bool ParseBool(string input)
    {
        if (string.Compare(input, "true", StringComparison.InvariantCultureIgnoreCase) == 0)
        {
            return true;
        }

        return false;
    }
}