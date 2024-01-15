using Microsoft.Extensions.Options;
using Tenon.Caching.Redis.Configurations;
using Tenon.Helper;
using Tenon.Helper.Internal;
using Tenon.Redis;
using Tenon.Serialization;

namespace Tenon.Caching.Redis;

public sealed class RedisCacheProvider(
    IRedisProvider redisProvider,
    ISerializer serializer,
    IOptionsMonitor<RedisCachingOptions> redisCachingOptions)
    : ICacheProvider
{
    private readonly RedisCachingOptions? _redisCachingOptions = redisCachingOptions.CurrentValue;

    public bool Set<T>(string cacheKey, T cacheValue, TimeSpan expiration)
    {
        ArgumentCheck(cacheKey, cacheValue, expiration);
        var cacheStringValue = GetCacheStringValue(cacheValue);
        expiration = GetExpiration(expiration);
        return redisProvider.StringSet(
            cacheKey,
            cacheStringValue,
            expiration);
    }

    public async Task<bool> SetAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration)
    {
        ArgumentCheck(cacheKey, cacheValue, expiration);
        var cacheStringValue = GetCacheStringValue(cacheValue);
        if (_redisCachingOptions?.MaxRandomSecond > 0)
            expiration =
                expiration.Add(TimeSpan.FromSeconds(RandomHelper.NextNumber(1, _redisCachingOptions.MaxRandomSecond)));
        return await redisProvider.StringSetAsync(
            cacheKey,
            cacheStringValue,
            expiration);
    }

    public CacheValue<T> Get<T>(string cacheKey)
    {
        ArgumentCheck(cacheKey);
        var result = redisProvider.StringGet(cacheKey);
        if (string.IsNullOrEmpty(result)) return CacheValue<T>.NoValue;
        var value = serializer.DeserializeObject<T>(result);
        return new CacheValue<T>(value, true);
    }

    public async Task<CacheValue<T>> GetAsync<T>(string cacheKey)
    {
        ArgumentCheck(cacheKey);
        var result = await redisProvider.StringGetAsync(cacheKey);
        if (string.IsNullOrEmpty(result)) return CacheValue<T>.NoValue;
        var value = serializer.DeserializeObject<T>(result);
        return new CacheValue<T>(value, true);
    }

    public void Remove(string cacheKey)
    {
        ArgumentCheck(cacheKey);
        redisProvider.KeyDelete(cacheKey);
    }

    public async Task RemoveAsync(string cacheKey)
    {
        ArgumentCheck(cacheKey);
        await redisProvider.KeyDeleteAsync(cacheKey);
    }

    public async Task<bool> ExistsAsync(string cacheKey)
    {
        ArgumentCheck(cacheKey);
        return await redisProvider.KeyExistsAsync(cacheKey);
    }

    public bool Exists(string cacheKey)
    {
        ArgumentCheck(cacheKey);
        return redisProvider.KeyExists(cacheKey);
    }

    public void RemoveAll(IEnumerable<string> cacheKeys)
    {
        redisProvider.KeyDelete(cacheKeys);
    }

    public async Task RemoveAllAsync(IEnumerable<string> cacheKeys)
    {
        await redisProvider.KeysDeleteAsync(cacheKeys);
    }

    public async Task KeysExpireAsync(IEnumerable<string> cacheKeys)
    {
        await redisProvider.KeysExpireAsync(cacheKeys);
    }

    private void ArgumentCheck(string cacheKey)
    {
        Checker.Begin()
            .NotNullOrEmpty(cacheKey, nameof(cacheKey));
    }

    private TimeSpan GetExpiration(TimeSpan expiration)
    {
        if (_redisCachingOptions?.MaxRandomSecond > 0)
            expiration =
                expiration.Add(TimeSpan.FromSeconds(RandomHelper.NextNumber(1, _redisCachingOptions.MaxRandomSecond)));
        return expiration;
    }

    private string? GetCacheStringValue<T>(T cacheValue)
    {
        return cacheValue is string ? cacheValue.ToString() : serializer.SerializeObject(cacheValue);
    }

    private static void ArgumentCheck<T>(string cacheKey, T cacheValue, TimeSpan expiration)
    {
        Checker.Begin()
            .NotNullOrEmpty(cacheKey, nameof(cacheKey))
            .NotNull(cacheValue, nameof(cacheValue));
        if (expiration <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(expiration));
    }
}