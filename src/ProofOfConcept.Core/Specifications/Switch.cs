namespace ProofOfConcept.Core.Specifications;

public record Switch(Location From, Gate Gate, List<Guard> Guards, UpdateMapping? UpdateMapping, Location To);