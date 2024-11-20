using ProofOfConcept.Core.Specifications;

namespace ProofOfConcept.Infrastructure.Microservices.Protocol.Redis;

public interface IRedisClient
{
    // TODO: Could use connection string as variable
    // TODO: Maybe a trace too
    Task<Trace> FlushAllData();
}