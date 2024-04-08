using Tenon.Caching.Abstractions;
using Tenon.Caching.Abstractions.Configurations;
using Tenon.Helper;
using Tenon.Helper.Internal;
using Tenon.Infra.Redis;
using Tenon.Serialization.Abstractions;

namespace Tenon.Caching.Redis;

public sealed class RedisCacheProvider
    : ICacheProvider
{
    private readonly CachingOptions _redisCachingOptions;
    private readonly IRedisProvider _redisProvider;
    private readonly ISerializer _serializer;

    public RedisCacheProvider(IRedisProvider redisProvider,
        ISerializer serializer, CachingOptions redisCachingOptions)
    {
        _redisCachingOptions = redisCachingOptions ?? throw new ArgumentNullException(nameof(redisCachingOptions));
        _redisProvider = redisProvider ?? throw new ArgumentNullException(nameof(redisProvider));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    }

    public bool Set<T>(string cacheKey, T cacheValue, TimeSpan expiration)
    {
        ArgumentCheck(cacheKey, cacheValue, expiration);
        var cacheStringValue = GetCacheStringValue(cacheValue);
        expiration = ReCalculateExpiration(expiration);
        return _redisProvider.StringSet(
            ReNameCacheKey(cacheKey),
            cacheStringValue,
            expiration);
    }

    public async Task<bool> SetAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration)
    {
        ArgumentCheck(cacheKey, cacheValue, expiration);
        var cacheStringValue = GetCacheStringValue(cacheValue);
        expiration = ReCalculateExpiration(expiration);
        return await _redisProvider.StringSetAsync(
            ReNameCacheKey(cacheKey),
            cacheStringValue,
            expiration);
    }

    public CacheValue<T> Get<T>(string cacheKey)
    {
        ArgumentCheck(cacheKey);
        var result = _redisProvider.StringGet(ReNameCacheKey(cacheKey));
        if (string.IsNullOrEmpty(result)) return CacheValue<T>.NoValue;
        var value = _serializer.DeserializeObject<T>(result);
        return new CacheValue<T>(value, true);
    }

    public async Task<CacheValue<T>> GetAsync<T>(string cacheKey)
    {
        ArgumentCheck(cacheKey);
        var result = await _redisProvider.StringGetAsync(ReNameCacheKey(cacheKey));
        if (string.IsNullOrEmpty(result)) return CacheValue<T>.NoValue;
        var value = _serializer.DeserializeObject<T>(result);
        return new CacheValue<T>(value, true);
    }

    public bool Remove(string cacheKey)
    {
        ArgumentCheck(cacheKey);
        return _redisProvider.KeyDelete(ReNameCacheKey(cacheKey));
    }

    public async Task<bool> RemoveAsync(string cacheKey)
    {
        ArgumentCheck(cacheKey);
        return await _redisProvider.KeyDeleteAsync(ReNameCacheKey(cacheKey));
    }

    public async Task<bool> ExistsAsync(string cacheKey)
    {
        ArgumentCheck(cacheKey);
        return await _redisProvider.KeyExistsAsync(ReNameCacheKey(cacheKey));
    }

    public bool Exists(string cacheKey)
    {
        ArgumentCheck(cacheKey);
        return _redisProvider.KeyExists(ReNameCacheKey(cacheKey));
    }

    public long RemoveAll(IEnumerable<string> cacheKeys)
    {
        ArgumentCheck(cacheKeys);
        return _redisProvider.KeyDelete(ReNameCacheKeys(cacheKeys));
    }

    public async Task<long> RemoveAllAsync(IEnumerable<string> cacheKeys)
    {
        ArgumentCheck(cacheKeys);
        return await _redisProvider.KeysDeleteAsync(ReNameCacheKeys(cacheKeys));
    }

    public async Task KeysExpireAsync(IEnumerable<string> cacheKeys)
    {
        ArgumentCheck(cacheKeys);
        await _redisProvider.KeysExpireAsync(ReNameCacheKeys(cacheKeys));
    }

    public async Task KeysExpireAsync(IEnumerable<string> cacheKeys, TimeSpan expiration)
    {
     
    }

    private string ReNameCacheKey(string cacheKey)
    {
        return string.IsNullOrWhiteSpace(_redisCachingOptions.Prefix)
            ? cacheKey
            : $"{_redisCachingOptions.Prefix}_{cacheKey}";
    }

    private IEnumerable<string> ReNameCacheKeys(IEnumerable<string> cacheKeys)
    {
        if (string.IsNullOrWhiteSpace(_redisCachingOptions.Prefix))
            return cacheKeys;
        return cacheKeys.Select(c => $"{_redisCachingOptions.Prefix}_{c}");
    }

    private void ArgumentCheck(IEnumerable<string> cacheKeys)
    {
        if (!(cacheKeys?.Any() ?? false)) throw new ArgumentNullException(nameof(cacheKeys));
        if (cacheKeys.Any(string.IsNullOrWhiteSpace))
            throw new ArgumentException($"{nameof(cacheKeys)} null value exists");
    }

    private TimeSpan ReCalculateExpiration(TimeSpan expiration)
    {
        return _redisCachingOptions?.MaxRandomSecond > 0
            ? expiration.Add(TimeSpan.FromSeconds(RandomHelper.NextNumber(1, _redisCachingOptions.MaxRandomSecond)))
            : expiration;
    }

    private void ArgumentCheck(string cacheKey)
    {
        Checker.Begin()
            .NotNullOrEmpty(cacheKey, nameof(cacheKey));
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