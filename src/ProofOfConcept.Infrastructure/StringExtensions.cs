namespace ProofOfConcept.Infrastructure;

public static class StringExtensions
{
    public static bool IsInputLabel(this string input)
    {
        return input.StartsWith("?");
    }
    
    public static bool IsOutputLabel(this string input)
    {
        return input.StartsWith("!");
    }
    
    public static bool IsAsyncOutputLabel(this string input)
    {
        return input.StartsWith("!@");
    }
}