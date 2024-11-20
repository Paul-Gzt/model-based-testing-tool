namespace ProofOfConcept.Core.Specifications;

public static class SwitchExtensions
{
    public static List<Switch> GetOutputActionChains(this Switch inputSwitch, Specification specification)
    {
        var result = new List<Switch>();

        var current = inputSwitch;

        while (current is not null)
        {
            current = specification
                .Switches
                .FirstOrDefault(s => s.From == current.To &&
                                     s.Gate.ActionType == ActionType.Output);
            if (current is null) continue;
            result.Add(current);
        }
        
        return result;
    }
}