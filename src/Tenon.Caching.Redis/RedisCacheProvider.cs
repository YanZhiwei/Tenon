using Microsoft.Extensions.Options;
using Tenon.Caching.Redis.Configurations;
using Tenon.Helper;
using Tenon.Helper.Internal;
using Tenon.Infra.Redis;
using Tenon.Serialization.Abstractions;

namespace Tenon.Caching.Redis;

public sealed class RedisCacheProvider
    : ICacheProvider
{
    private readonly RedisCachingOptions _redisCachingOptions;
    private readonly IRedisProvider _redisProvider;
    private readonly ISerializer _serializer;

    public RedisCacheProvider(RedisCachingOptions redisCachingOptions, IRedisProvider redisProvider,
        ISerializer serializer)
    {
        _redisCachingOptions = redisCachingOptions ?? throw new ArgumentNullException(nameof(redisCachingOptions));
        _redisProvider = redisProvider ?? throw new ArgumentNullException(nameof(redisProvider));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    }

    public RedisCacheProvider(IOptionsMonitor<RedisCachingOptions> redisCachingOptions, IRedisProvider redisProvider,
        ISerializer serializer)
    {
        var redisCachingOptionsMonitor =
            redisCachingOptions ?? throw new ArgumentNullException(nameof(redisCachingOptions));
        _redisProvider = redisProvider ?? throw new ArgumentNullException(nameof(redisProvider));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _redisCachingOptions = redisCachingOptionsMonitor.CurrentValue;
    }

    public bool Set<T>(string cacheKey, T cacheValue, TimeSpan expiration)
    {
        ArgumentCheck(cacheKey, cacheValue, expiration);
        var cacheStringValue = GetCacheStringValue(cacheValue);
        expiration = GetExpiration(expiration);
        return _redisProvider.StringSet(
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
        return await _redisProvider.StringSetAsync(
            cacheKey,
            cacheStringValue,
            expiration);
    }

    public CacheValue<T> Get<T>(string cacheKey)
    {
        ArgumentCheck(cacheKey);
        var result = _redisProvider.StringGet(cacheKey);
        if (string.IsNullOrEmpty(result)) return CacheValue<T>.NoValue;
        var value = _serializer.DeserializeObject<T>(result);
        return new CacheValue<T>(value, true);
    }

    public async Task<CacheValue<T>> GetAsync<T>(string cacheKey)
    {
        ArgumentCheck(cacheKey);
        var result = await _redisProvider.StringGetAsync(cacheKey);
        if (string.IsNullOrEmpty(result)) return CacheValue<T>.NoValue;
        var value = _serializer.DeserializeObject<T>(result);
        return new CacheValue<T>(value, true);
    }

    public void Remove(string cacheKey)
    {
        ArgumentCheck(cacheKey);
        _redisProvider.KeyDelete(cacheKey);
    }

    public async Task RemoveAsync(string cacheKey)
    {
        ArgumentCheck(cacheKey);
        await _redisProvider.KeyDeleteAsync(cacheKey);
    }

    public async Task<bool> ExistsAsync(string cacheKey)
    {
        ArgumentCheck(cacheKey);
        return await _redisProvider.KeyExistsAsync(cacheKey);
    }

    public bool Exists(string cacheKey)
    {
        ArgumentCheck(cacheKey);
        return _redisProvider.KeyExists(cacheKey);
    }

    public void RemoveAll(IEnumerable<string> cacheKeys)
    {
        _redisProvider.KeyDelete(cacheKeys);
    }

    public async Task RemoveAllAsync(IEnumerable<string> cacheKeys)
    {
        await _redisProvider.KeysDeleteAsync(cacheKeys);
    }

    public async Task KeysExpireAsync(IEnumerable<string> cacheKeys)
    {
        await _redisProvider.KeysExpireAsync(cacheKeys);
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
        return cacheValue is string ? cacheValue.ToString() : _serializer.SerializeObject(cacheValue);
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