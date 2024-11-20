namespace ProofOfConcept.Core.Solver;

public readonly record struct Variable(string Name, string Value, Type Type)
{
    public static Variable Empty => new("Uninitialized", "null", typeof(object));

    public bool IsUndefined()
    {
        return Name == "Uninitialized";
    }
}