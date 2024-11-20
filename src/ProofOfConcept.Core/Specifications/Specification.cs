using System.Collections.Immutable;
using ProofOfConcept.Core.Solver;

namespace ProofOfConcept.Core.Specifications;

public class Specification
{
    /// <summary>
    /// The initial starting location of this <see cref="Specification"/>
    /// </summary>
    public Location InitialLocation { get; init; }

    /// <summary>
    /// The initial values of this <see cref="Specification"/> when it was created.
    /// </summary>    
    public ImmutableList<Variable> InitialVariables { get; init; } = new List<Variable>().ToImmutableList();
    
    /// <summary>
    /// All Locations within this <see cref="Specification"/>
    /// </summary>
    public List<Location> Locations { get; init; }  = new();

    /// <summary>
    /// All labeled actions defined in this <see cref="Specification"/>.
    /// </summary>
    public List<Gate> Gates { get; init; } = new();

    /// <summary>
    /// All possible Transitions in this <see cref="Specification"/>
    /// </summary>
    public List<Switch> Switches { get; init; } = new();

    /// <summary>
    /// Contains all aspects that define the state of this <see cref="Specification"/>.
    /// </summary>
    public SpecificationState CurrentState { get; set; } = SpecificationState.Empty;
    
    /// <summary>
    /// Performs switches if possible and updates internal states and variables
    /// TODO: Logging or different results when transitioning is not possible?
    /// TODO: Handle non-determinism: we now always choose the first transition
    /// TODO: Variable update mapping
    /// </summary>
    /// <param name="gate"></param>
    public (bool, List<Gate>) Switch(Gate gate)
    {
        var reachableSwitches = Switches
            .Where(@switch => CurrentState.InstantiatedLocations.Select(location => location.Name).Contains(@switch.From.Name) &&
                        @switch.Gate.Label == gate.Label &&
                        @switch.Gate.ActionType == gate.ActionType)
            .ToList();
        
        if (!reachableSwitches.Any())
        {
            // TODO: This will always be empty
            var expectedGates = reachableSwitches.Select(@switch => @switch.Gate).ToList();
            return (false, expectedGates);
        }

        var currentInstantiatedLocations = reachableSwitches.Select(x => x.To).ToList();
        CurrentState = CurrentState.Update(currentInstantiatedLocations);

        return (true, new List<Gate>());
    }
}