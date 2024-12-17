using ProofOfConcept.Core.Specifications;

namespace ProofOfConcept.Core.Composition;

public static class ParallelComposition
{
    public static List<(Switch switchA, Switch switchB)> FindSynchronizingActions(Specification a, Specification b)
    {
        var switchesA = a.Switches
            .Where(x => x.Gate.ActionType == ActionType.Output)
            .ToList();

        var switchesB = b.Switches
            .Where(x => x.Gate.ActionType == ActionType.Input)
            .ToList();
        
        var synchronizingGatesA = GetSwitchesAndGuards(switchesA);
        var synchronizingGatesB = GetSwitchesAndGuards(switchesB);
	
	foreach (var foo in synchronizingGatesA) {
		Console.WriteLine("{0} {1}", foo.Switch.Gate.Label, foo.RightOperandLabel);
	}

	Console.WriteLine();

	foreach (var foo in synchronizingGatesB) {
		Console.WriteLine("{0} {1}", foo.Switch.Gate.Label, foo.RightOperandLabel);
	}

        var intersectedGates = 
            from gateA in synchronizingGatesA
            join gateB in synchronizingGatesB on gateA.RightOperandLabel equals gateB.RightOperandLabel
            select gateA;

        return intersectedGates
            .Select(g =>
            {
                bool Predicate(Switch @switch) => RemoveAction(@switch.Gate.Label) == RemoveAction(g.Switch.Gate.Label) && @switch.Guards.Any(guard => GetUrl(guard.RightOperand) == g.RightOperandLabel);

                return (
                    a.Switches.First(Predicate),
                    b.Switches.First(Predicate)
                );
            }).ToList();
    }

    private static List<SwitchAndGuard> GetSwitchesAndGuards(List<Switch> switches)
    {
        return (from @switch in switches from guard in @switch.Guards select new SwitchAndGuard(@switch, GetUrl(guard.RightOperand))).ToList();
    }

    private static string GetUrl(string operand)
    {
        var url = operand.Split("::");

        return url.Length < 2 ? operand : url[1];
    }
    // TODO: Never put the action in the specification label?
    private static string RemoveAction(string input)
    {
        return input.Substring(1);
    }
}

internal record SwitchAndGuard(Switch @Switch, string RightOperandLabel);
