using StackExchange.Redis;

namespace Tenon.Redis.StackExchangeProvider;

public partial class StackExchangeProvider
{
    public long IncrBy(string cacheKey, long value = 1)
    {
        CheckCacheKey(cacheKey);
        return _redisDatabase.StringIncrement(cacheKey, value);
    }

    public async Task<long> IncrByAsync(string cacheKey, long value = 1)
    {
        CheckCacheKey(cacheKey);
        return await _redisDatabase.StringIncrementAsync(cacheKey, value);
    }

    public double IncrByFloat(string cacheKey, double value = 1)
    {
        CheckCacheKey(cacheKey);
        return _redisDatabase.StringIncrement(cacheKey, value);
    }

    public async Task<double> IncrByFloatAsync(string cacheKey, double value = 1)
    {
        CheckCacheKey(cacheKey);
        return await _redisDatabase.StringIncrementAsync(cacheKey, value);
    }

    public bool StringSet(string cacheKey, string cacheValue, TimeSpan? expiration = null,
        StringSetWhen setWhen = StringSetWhen.Always)
    {
        CheckCacheKey(cacheKey);
        var w = setWhen switch
        {
            StringSetWhen.NotExists => When.NotExists,
            StringSetWhen.Exists => When.Exists,
            _ => When.Always
        };
        return _redisDatabase.StringSet(cacheKey, cacheValue, expiration, w);
    }

    public async Task<bool> StringSetAsync(string cacheKey, string cacheValue, TimeSpan? expiration = null,
        StringSetWhen setWhen = StringSetWhen.Always)
    {
        CheckCacheKey(cacheKey);
        var w = setWhen switch
        {
            StringSetWhen.NotExists => When.NotExists,
            StringSetWhen.Exists => When.Exists,
            _ => When.Always
        };
        return await _redisDatabase.StringSetAsync(cacheKey, cacheValue, expiration, w);
    }

    public string? StringGet(string cacheKey)
    {
        CheckCacheKey(cacheKey);
        return _redisDatabase.StringGet(cacheKey);
    }

    public async Task<string?> StringGetAsync(string cacheKey)
    {
        CheckCacheKey(cacheKey);
        return await _redisDatabase.StringGetAsync(cacheKey);
    }

    public long StringLen(string cacheKey)
    {
        CheckCacheKey(cacheKey);
        return _redisDatabase.StringLength(cacheKey);
    }

    public async Task<long> StringLenAsync(string cacheKey)
    {
        CheckCacheKey(cacheKey);
        return await _redisDatabase.StringLengthAsync(cacheKey);
    }

    public long StringSetRange(string cacheKey, long offset, string value)
    {
        CheckCacheKey(cacheKey);
        var res = _redisDatabase.StringSetRange(cacheKey, offset, value);
        return (long)res;
    }

    public async Task<long> StringSetRangeAsync(string cacheKey, long offset, string value)
    {
        CheckCacheKey(cacheKey);
        var res = await _redisDatabase.StringSetRangeAsync(cacheKey, offset, value);
        return (long)res;
    }

    public string? StringGetRange(string cacheKey, long start, long end)
    {
        CheckCacheKey(cacheKey);
        return _redisDatabase.StringGetRange(cacheKey, start, end);
    }

    public async Task<string?> StringGetRangeAsync(string cacheKey, long start, long end)
    {
        CheckCacheKey(cacheKey);
        return await _redisDatabase.StringGetRangeAsync(cacheKey, start, end);
    }
}