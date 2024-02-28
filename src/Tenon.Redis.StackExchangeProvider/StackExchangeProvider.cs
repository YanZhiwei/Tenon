using StackExchange.Redis;
using Tenon.Helper;
using Tenon.Serialization;

namespace Tenon.Redis.StackExchangeProvider;

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

    public StackExchangeProvider(RedisConnection redisConnection) : this(redisConnection, null)
    {
    }

    private void CheckCacheKey(string cacheKey)
    {
        Checker.Begin().NotNullOrEmpty(cacheKey, nameof(cacheKey));
    }
}