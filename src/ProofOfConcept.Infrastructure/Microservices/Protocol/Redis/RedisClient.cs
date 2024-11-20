using ProofOfConcept.Core.Specifications;
using StackExchange.Redis;

namespace ProofOfConcept.Infrastructure.Microservices.Protocol.Redis;

public class RedisClient : IRedisClient
{
    public async Task<Trace> FlushAllData()
    {
        var redis = await ConnectionMultiplexer.ConnectAsync("localhost,allowAdmin=true,password=Geheim_123");
        var server = redis.GetServer("localhost:6379");
        await server.FlushDatabaseAsync();

        return new Trace(new Gate(ActionType.Input, "!redis_flushed"), string.Empty);
    }
}