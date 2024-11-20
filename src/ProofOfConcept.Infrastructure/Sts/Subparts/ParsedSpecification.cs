namespace ProofOfConcept.Infrastructure.Sts.Subparts;

public record ParsedSpecification(GlobalVariables GlobalVariables, string? InitialLocation, List<Switch> Switches);