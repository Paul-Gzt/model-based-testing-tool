namespace Basket.API;

public static class Mutant
{
    private static readonly string _path;
    
    static Mutant()
    {
        _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mutants", "active_mutants.txt");
    }

    public static bool IsActive(int number)
    {
        var fileLines = File.ReadAllText(_path);
        var res = int.Parse(fileLines);

        return res == number;
    }
}