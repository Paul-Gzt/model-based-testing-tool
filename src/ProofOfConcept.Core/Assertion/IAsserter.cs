using ProofOfConcept.Core.Specifications;

namespace ProofOfConcept.Core.Assertion;

public interface IAsserter
{
    Task<AssertionResult> AssertAsync(string value, Switch @switch);
}