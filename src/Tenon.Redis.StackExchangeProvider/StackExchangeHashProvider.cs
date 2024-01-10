using StackExchange.Redis;

namespace Tenon.Redis.StackExchangeProvider;

public partial class StackExchangeProvider
{
    public bool HMSet(string cacheKey, IDictionary<string, string> vals, TimeSpan? expiration = null)
    {
        if (expiration.HasValue)
        {
            _redisDatabase.HashSet(cacheKey, vals.Select(item => new HashEntry(item.Key, item.Value)).ToArray());
            return _redisDatabase.KeyExpire(cacheKey, expiration);
        }

        _redisDatabase.HashSet(cacheKey, vals.Select(item => new HashEntry(item.Key, item.Value)).ToArray());
        return true;
    }

    public bool HSet(string cacheKey, string field, string cacheValue)
    {
        return _redisDatabase.HashSet(cacheKey, field, cacheValue);
    }

    public bool HExists(string cacheKey, string field)
    {
        return _redisDatabase.HashExists(cacheKey, field);
    }

    public long HDel(string cacheKey, IEnumerable<string>? fields = null)
    {
        if (fields?.Any() ?? false)
            return _redisDatabase.HashDelete(cacheKey, fields.Select(x => (RedisValue)x).ToArray());

        var flag = _redisDatabase.KeyDelete(cacheKey);
        return flag ? 1 : 0;
    }

    public string? HGet(string cacheKey, string field)
    {
        return _redisDatabase.HashGet(cacheKey, field);
    }

    public IDictionary<string, string> HGetAll(string cacheKey)
    {
        var dict = new Dictionary<string, string>();
        var vals = _redisDatabase.HashGetAll(cacheKey);
        foreach (var item in vals)
            if (!dict.ContainsKey(item.Name))
                dict.Add(item.Name, item.Value);
        return dict;
    }

    public long HIncrBy(string cacheKey, string field, long val = 1)
    {
        return _redisDatabase.HashIncrement(cacheKey, field, val);
    }

    public IEnumerable<string> HKeys(string cacheKey)
    {
        var keys = _redisDatabase.HashKeys(cacheKey);
        return keys.Select(x => x.ToString()).ToList();
    }

    public long HLen(string cacheKey)
    {
        return _redisDatabase.HashLength(cacheKey);
    }

    public IEnumerable<string> HVals(string cacheKey)
    {
        return _redisDatabase.HashValues(cacheKey).Select(x => x.ToString()).ToList();
    }

    public IDictionary<string, string> HMGet(string cacheKey, IEnumerable<string> fields)
    {
        var dict = new Dictionary<string, string>();
        var list = _redisDatabase.HashGet(cacheKey, fields.Select(x => (RedisValue)x).ToArray());
        for (var i = 0; i < fields.Count(); i++)
        {
            var key = fields.ElementAt(i);
            if (!dict.ContainsKey(key))
                dict.Add(key, list.ElementAt(i));
        }

        return dict;
    }

    public async Task<bool> HMSetAsync(string cacheKey, IDictionary<string, string> vals, TimeSpan? expiration = null)
    {
        if (expiration.HasValue)
        {
            await _redisDatabase.HashSetAsync(cacheKey,
                vals.Select(item => new HashEntry(item.Key, item.Value)).ToArray());
            return await _redisDatabase.KeyExpireAsync(cacheKey, expiration);
        }

        await _redisDatabase.HashSetAsync(cacheKey, vals.Select(item => new HashEntry(item.Key, item.Value)).ToArray());
        return true;
    }

    public async Task<bool> HSetAsync(string cacheKey, string field, string cacheValue)
    {
        return await _redisDatabase.HashSetAsync(cacheKey, field, cacheValue);
    }

    public async Task<bool> HExistsAsync(string cacheKey, string field)
    {
        return await _redisDatabase.HashExistsAsync(cacheKey, field);
    }

    public async Task<long> HDelAsync(string cacheKey, IEnumerable<string>? fields = null)
    {
        if (fields?.Any() ?? false)
        {
            return await _redisDatabase.HashDeleteAsync(cacheKey, fields.Select(x => (RedisValue)x).ToArray());
        }

        var flag = await _redisDatabase.KeyDeleteAsync(cacheKey);
        return flag ? 1 : 0;
    }

    public async Task<string?> HGetAsync(string cacheKey, string field)
    {
       return await _redisDatabase.HashGetAsync(cacheKey, field);
    }

    public async Task<IDictionary<string, string>> HGetAllAsync(string cacheKey)
    {
        var dict = new Dictionary<string, string>();
        var vals = await _redisDatabase.HashGetAllAsync(cacheKey);
        foreach (var item in vals)
        {
            var key = item.Name;
            if (!dict.ContainsKey(key)) 
                dict.Add(key, item.Value);
        }
        return dict;
    }

    public async Task<long> HIncrByAsync(string cacheKey, string field, long val = 1)
    {
        return await _redisDatabase.HashIncrementAsync(cacheKey, field, val);
    }

    public async Task<IEnumerable<string>> HKeysAsync(string cacheKey)
    {
        var keys = await _redisDatabase.HashKeysAsync(cacheKey);
        return keys.Select(x => x.ToString()).ToList();
    }

    public async Task<long> HLenAsync(string cacheKey)
    {
        return await _redisDatabase.HashLengthAsync(cacheKey);
    }

    public async Task<IEnumerable<string>> HValsAsync(string cacheKey)
    {
        return (await _redisDatabase.HashValuesAsync(cacheKey)).Select(x => x.ToString()).ToList();
    }

    public async Task<IDictionary<string, string>> HMGetAsync(string cacheKey, IEnumerable<string> fields)
    {
        var dict = new Dictionary<string, string>();

        var res = await _redisDatabase.HashGetAsync(cacheKey, fields.Select(x => (RedisValue)x).ToArray());

        for (var i = 0; i < fields.Count(); i++)
        {
            var key = fields.ElementAt(i);
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, res.ElementAt(i));
            }
        }

        return dict;
    }
}