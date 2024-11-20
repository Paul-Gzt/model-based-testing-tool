using ProofOfConcept.Infrastructure.Sts.Subparts;

namespace ProofOfConcept.Infrastructure.Sts.CompletionAlgorithms;

public interface ICompletionAlgorithm
{
    ParsedSpecification PerformCompletion(ParsedSpecification result);
}