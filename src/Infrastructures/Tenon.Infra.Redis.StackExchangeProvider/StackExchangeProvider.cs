using StackExchange.Redis;
using Tenon.Helper;
using Tenon.Infra.Redis.Configurations;
using Tenon.Serialization.Abstractions;

namespace Tenon.Infra.Redis.StackExchangeProvider;

public partial class StackExchangeProvider : IRedisProvider
{
    private readonly IDatabase _redisDatabase;
    private readonly ISerializer? _serializer;

    public StackExchangeProvider(RedisConnection redisConnection, ISerializer? serializer)
    {
        _serializer = serializer;
        redisConnection = redisConnection ?? throw new ArgumentNullException(nameof(redisConnection));
        _redisDatabase = redisConnection.GetDatabase();
    }

    public StackExchangeProvider(RedisOptions redisOptions, ISerializer? serializer)
    {
        _serializer = serializer;
        var redisConnection = new RedisConnection(redisOptions);
        _redisDatabase = redisConnection.GetDatabase();
    }

    public StackExchangeProvider(RedisOptions redisOptions) : this(redisOptions, null)
    {
    }

    public StackExchangeProvider(RedisConnection redisConnection) : this(redisConnection, null)
    {
    }

    private void CheckCacheKey(string cacheKey)
    {
        Checker.Begin().NotNullOrEmpty(cacheKey, nameof(cacheKey));
    }
}