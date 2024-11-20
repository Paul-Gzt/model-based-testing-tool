using ProofOfConcept.Core.Specifications;

namespace ProofOfConcept.Core.Testing;

public interface ISystemUnderTest
{
    Task SendInput(string label, List<Parameter> parameters);

    Trace ReceiveOutput();
}