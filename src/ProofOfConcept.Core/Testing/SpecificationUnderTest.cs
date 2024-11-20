using ProofOfConcept.Core.Assertion;
using ProofOfConcept.Core.Specifications;

namespace ProofOfConcept.Core.Testing;

public class SpecificationUnderTest
{
    private readonly Specification _specification;

    private readonly IAsserter _asserter;

    public SpecificationState CurrentState;

    public SpecificationUnderTest(Specification specification, SpecificationState specificationState, IAsserter asserter)
    {
        _specification = specification;
        CurrentState = specificationState;
        _asserter = asserter;
    }

    public async Task SwitchInput(Gate gate)
    {
        await Switch(gate);
    }

    public async Task<SwitchResult> SwitchOutput(Gate gate, string data)
    {
        return await Switch(gate, data);
    }
    
    /// <summary>
    /// Performs switches if possible and updates internal states and variables
    /// TODO: Variable update mapping
    /// </summary>
    /// <param name="gate"></param>
    /// <param name="data"></param>
    private async Task<SwitchResult> Switch(Gate gate, string? data = null)
    {
        var reachableSwitches = _specification.Switches
            .Where(@switch => CurrentState.InstantiatedLocations.Select(location => location.Name).Contains(@switch.From.Name) &&
                              @switch.Gate.Label == gate.Label &&
                              @switch.Gate.ActionType == gate.ActionType)
            .ToList();
        
        if (!reachableSwitches.Any())
        {
            var expectedGates = reachableSwitches.Select(@switch => @switch.Gate).ToList();
            return new SwitchResult(false, expectedGates);
        }

        if (data is not null)
        {
            var evaluatedSwitches = new List<Switch>();
            var errors = new List<Switch>();
            foreach (var reachableSwitch in reachableSwitches)
            {
                var assertion = await _asserter.AssertAsync(data, reachableSwitch);

                if (assertion.Success)
                {
                    evaluatedSwitches.Add(reachableSwitch);
                }
                else
                {
                    Console.WriteLine("Assertion failed. Reason: '{0}'", assertion.Reason);
                    errors.Add(reachableSwitch);
                }
            }

            if (evaluatedSwitches.Any())
            {
                var evaluatedLocations = evaluatedSwitches.Select(x => x.To).ToList();
                CurrentState = CurrentState.Update(evaluatedLocations);
                return new SwitchResult(true, new List<Gate>());    
            }

            // TODO: Return expected gates, log errors
            return new SwitchResult(false, new List<Gate>());
        }

        var currentInstantiatedLocations = reachableSwitches.Select(x => x.To).ToList();
        CurrentState = CurrentState.Update(currentInstantiatedLocations);

        return new SwitchResult(true, new List<Gate>());
    }
    
    // /// <summary>
    // /// Gets all switches that can be instantiated using the specification's current state
    // /// </summary>
    // /// <returns></returns>
    // public List<Switch> GetPossibleInstantiatedSwitches()
    // {
    //     return Switches
    //         .Where(s => CurrentState.InstantiatedLocations.Contains(s.From))
    //         .ToList();
    // }
}

public readonly record struct SwitchResult(bool IsExpected, List<Gate> Expected); 