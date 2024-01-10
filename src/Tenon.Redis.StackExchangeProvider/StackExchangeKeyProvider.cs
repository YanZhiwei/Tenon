namespace Tenon.Redis.StackExchangeProvider;

public partial class StackExchangeProvider
{
    public bool KeyDelete(string cacheKey)
    {
        return _redisDatabase.KeyDelete(cacheKey);
    }

    public async Task<bool> KeyDeleteAsync(string cacheKey)
    {
        return await _redisDatabase.KeyDeleteAsync(cacheKey);
    }

    public bool KeyExpire(string cacheKey, int second)
    {
        return _redisDatabase.KeyExpire(cacheKey, TimeSpan.FromSeconds(second));
    }

    public async Task<bool> KeyExpireAsync(string cacheKey, int second)
    {
        return await _redisDatabase.KeyExpireAsync(cacheKey, TimeSpan.FromSeconds(second));
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