namespace ProofOfConcept.Infrastructure.Microservices;

public interface IAdapter
{
    void SetNext(IAdapter adapter);
}