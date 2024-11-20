using ProofOfConcept.Core.Specifications;
using ProofOfConcept.Core.Testing;
using ProofOfConcept.Infrastructure.Microservices.Protocol.Redis;

namespace ProofOfConcept.Infrastructure.Microservices.Input;

public class RedisInputAdapter : InputAdapterBase
{
    private static readonly List<string> SupportedLabels = new()
    {
        "?redis_flush_all_data"
    };

    private readonly IRedisClient _redisClient;
    
    public RedisInputAdapter(IRedisClient redisClient) : base(SupportedLabels)
    {
        _redisClient = redisClient;
    }

    protected override async Task<Trace?> DoAction(string label, List<Parameter> parameters)
    {
        return label switch
        {
            "?redis_flush_all_data" => await _redisClient.FlushAllData(),
            _ => throw new NotImplementedException($"Unsupported label: {label}")
        };
    }
}