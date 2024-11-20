using ProofOfConcept.Core.Specifications;

namespace ProofOfConcept.Infrastructure.Microservices.Protocol.Http;

public interface IHttpClient
{
    Task<Trace> GetAsync(string endpoint);

    Task<Trace> PostAsync(string endpoint, string body);

    Task<Trace> PutAsync(string endpoint, string body);

    Task<Trace> DeleteAsync(string endpoint);
}