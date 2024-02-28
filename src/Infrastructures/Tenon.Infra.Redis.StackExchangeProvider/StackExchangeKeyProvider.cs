using StackExchange.Redis;

namespace Tenon.Infra.Redis.StackExchangeProvider;

public partial class StackExchangeProvider
{
    public bool KeyDelete(string cacheKey)
    {
        return _redisDatabase.KeyDelete(cacheKey);
    }

    public long KeyDelete(IEnumerable<string> cacheKeys)
    {
        var redisKeys = cacheKeys.Where(k => !string.IsNullOrEmpty(k)).Select(k => (RedisKey)k).ToArray();
        return redisKeys.Any() ? _redisDatabase.KeyDelete(redisKeys) : 0;
    }

    public async Task<bool> KeyDeleteAsync(string cacheKey)
    {
        return await _redisDatabase.KeyDeleteAsync(cacheKey);
    }

    public async Task<long> KeysDeleteAsync(IEnumerable<string> cacheKeys)
    {
        var redisKeys = cacheKeys.Where(k => !string.IsNullOrEmpty(k)).Select(k => (RedisKey)k).ToArray();
        return redisKeys.Any() ? await _redisDatabase.KeyDeleteAsync(redisKeys) : 0;
    }

    public bool KeyExpire(string cacheKey, int second)
    {
        return _redisDatabase.KeyExpire(cacheKey, TimeSpan.FromSeconds(second));
    }

    public async Task<bool> KeyExpireAsync(string cacheKey, int second)
    {
        return await _redisDatabase.KeyExpireAsync(cacheKey, TimeSpan.FromSeconds(second));
    }

    public async Task<long> KeysExpireAsync(IEnumerable<string> cacheKeys)
    {
        var redisKeys = cacheKeys.Where(k => !string.IsNullOrEmpty(k)).Select(k => (RedisKey)k).ToArray();
        return redisKeys.Any() ? await _redisDatabase.KeyExistsAsync(redisKeys) : 0;
    }

    public async Task<bool> KeyExistsAsync(string cacheKey)
    {
        return await _redisDatabase.KeyExistsAsync(cacheKey);
    }

    public bool KeyExists(string cacheKey)
    {
        return _redisDatabase.KeyExists(cacheKey);
    }

    public long TTL(string cacheKey)
    {
        var ts = _redisDatabase.KeyTimeToLive(cacheKey);
        return ts.HasValue ? (long)ts.Value.TotalSeconds : -1;
    }

    public async Task<long> TTLAsync(string cacheKey)
    {
        var ts = await _redisDatabase.KeyTimeToLiveAsync(cacheKey);
        return ts.HasValue ? (long)ts.Value.TotalSeconds : -1;
    }
}