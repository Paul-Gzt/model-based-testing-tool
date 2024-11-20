namespace ProofOfConcept.Infrastructure.Sts.Subparts;

public record Variable(string Name, string Value)
{
    public Core.Solver.Variable Map()
    {
        if (int.TryParse(Value, out var valueAsInt))
        {
            return new Core.Solver.Variable(Name, valueAsInt.ToString(), typeof(int));
        }
        
        return new Core.Solver.Variable(Name, Value, typeof(string));
    }
}