using ProofOfConcept.Core.Solver;

namespace ProofOfConcept.Core.Specifications;

// TODO: Would be pretty if this can be inferred from the Specification
public record SpecificationState
{
    /// <summary>
    /// Global variables, disjoint from <see cref="InteractionVariables"/>.
    /// </summary>
    public List<Variable> LocationVariables { get; init; } = new();

    /// <summary>
    /// Local variables, disjoint from <see cref="LocationVariables"/>.
    /// </summary>
    public List<Variable> InteractionVariables { get; init; } = new();

    /// <summary>
    /// The current instantiated <see cref="Location"/>s.
    /// Due to non-determinism, this can be a list of possible locations.
    /// </summary>
    public List<Location> InstantiatedLocations { get; init; } = new();

    /// <summary>
    /// Updates all properties based on parameters
    /// </summary>
    /// <param name="currentInstantiatedLocations"></param>
    public SpecificationState Update(List<Location> currentInstantiatedLocations)
    {
        return this with { InstantiatedLocations = currentInstantiatedLocations };
    }

    public static SpecificationState Empty => new();
}