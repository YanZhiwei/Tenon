using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Tenon.Helper;
using Tenon.Infra.Redis.Configurations;
using Tenon.Serialization.Abstractions;

namespace Tenon.Infra.Redis.StackExchangeProvider;

public partial class StackExchangeProvider : IRedisProvider
{
    private readonly IDatabase _redisDatabase;
    private readonly ISerializer? _serializer;

    public StackExchangeProvider(RedisOptions redisOptions, ISerializer? serializer)
    {
        _serializer = serializer;
        var redisConnection = new RedisConnection(redisOptions);
        _redisDatabase = redisConnection.GetDatabase();
    }

    public StackExchangeProvider(IOptionsMonitor<RedisOptions> redisOptions, ISerializer? serializer)
    {
        _serializer = serializer;
        var redisConnection = new RedisConnection(redisOptions);
        _redisDatabase = redisConnection.GetDatabase();
    }

    private void CheckCacheKey(string cacheKey)
    {
        Checker.Begin().NotNullOrEmpty(cacheKey, nameof(cacheKey));
    }
}