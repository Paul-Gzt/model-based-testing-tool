using ProofOfConcept.Core.Specifications;

namespace ProofOfConcept.Infrastructure.Microservices.Protocol.Sql;

public interface ISqlClient
{
    // TODO: Give dynamic list of tables to clear
    Task<Trace> FlushAllData();

    Task<Trace> Query(string query);

    Task Seed(string seedScript);
}