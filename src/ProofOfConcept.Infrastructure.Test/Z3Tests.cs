using Microsoft.Z3;

namespace ProofOfConcept.Infrastructure.Test;

[TestClass]
public class Z3Tests
{
    [TestMethod]
    public void Test()
    {
        Console.WriteLine("SimpleExample");

        using Context ctx = new Context();
        
        Attempt1(ctx);
    }

    // solve x > 2 and y < 10 and x + 2y = 7
    public void Attempt1(Context context)
    {
        var x = context.MkIntConst("x");
        var y = context.MkIntConst("y");
        
        var solver = context.MkSolver();

        var formulaXGt2 = context.MkGt(x, context.MkInt(2));
        var formulaYLt10 = context.MkLt(y, context.MkInt(10)); 
        var formulaEq = context.MkEq(context.MkAdd(x, context.MkMul(y, context.MkInt(2))), context.MkInt(7));

        solver.Assert(formulaXGt2, formulaYLt10, formulaEq);

        var res = solver.Check();

        Console.WriteLine(res);

        var model = solver.Model;
        
        Console.WriteLine(model);
    }
    
}