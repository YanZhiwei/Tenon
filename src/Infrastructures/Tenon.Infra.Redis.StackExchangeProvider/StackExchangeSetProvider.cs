using StackExchange.Redis;

namespace Tenon.Infra.Redis.StackExchangeProvider;

public partial class StackExchangeProvider
{
    public long SAdd<T>(string cacheKey, IEnumerable<T> cacheValues, TimeSpan? expiration = null)
    {
        var list = new List<RedisValue>();

        foreach (var item in cacheValues)
            list.Add(_serializer.Serialize(item));

        var len = _redisDatabase.SetAdd(cacheKey, list.ToArray());

        if (expiration.HasValue) _redisDatabase.KeyExpire(cacheKey, expiration.Value);

        return len;
    }

    public long SCard(string cacheKey)
    {
        return _redisDatabase.SetLength(cacheKey);
    }

    public bool SIsMember<T>(string cacheKey, T cacheValue)
    {
        var bytes = _serializer.Serialize(cacheValue);
        return _redisDatabase.SetContains(cacheKey, bytes);
    }

    public List<T> SMembers<T>(string cacheKey)
    {
        var list = new List<T>();
        var bytes = _redisDatabase.SetMembers(cacheKey);
        foreach (var item in bytes) list.Add(_serializer.Deserialize<T>(item));
        return list;
    }

    public T SPop<T>(string cacheKey)
    {
        var bytes = _redisDatabase.SetPop(cacheKey);
        return _serializer.Deserialize<T>(bytes);
    }

    public IEnumerable<T> SRandMember<T>(string cacheKey, int count = 1)
    {
        var list = new List<T>();

        var bytes = _redisDatabase.SetRandomMembers(cacheKey, count);

        foreach (var item in bytes) list.Add(_serializer.Deserialize<T>(item));

        return list;
    }

    public long SRem<T>(string cacheKey, IEnumerable<T>? cacheValues = null)
    {
        if (cacheValues?.Any() ?? false)
        {
            var bytes = new List<RedisValue>();

            foreach (var item in cacheValues)
                bytes.Add(_serializer.Serialize(item));

            return _redisDatabase.SetRemove(cacheKey, bytes.ToArray());
        }

        var flag = _redisDatabase.KeyDelete(cacheKey);
        return flag ? 1 : 0;
    }

    public async Task<long> SAddAsync<T>(string cacheKey, IEnumerable<T> cacheValues, TimeSpan? expiration = null)
    {
        var list = new List<RedisValue>();

        foreach (var item in cacheValues)
        {
            list.Add(_serializer.Serialize(item));
        }

        var len = await _redisDatabase.SetAddAsync(cacheKey, list.ToArray());

        if (expiration.HasValue)
        {
            await _redisDatabase.KeyExpireAsync(cacheKey, expiration.Value);
        }

        return len;
    }

    public async Task<long> SCardAsync(string cacheKey)
    {
        return await _redisDatabase.SetLengthAsync(cacheKey);
    }

    public async Task<bool> SIsMemberAsync<T>(string cacheKey, T cacheValue)
    {
        var bytes = _serializer.Serialize(cacheValue);
        return await _redisDatabase.SetContainsAsync(cacheKey, bytes);
    }

    public async Task<IEnumerable<T>> SMembersAsync<T>(string cacheKey)
    {
        var list = new List<T>();

        var vals = await _redisDatabase.SetMembersAsync(cacheKey);

        foreach (var item in vals)
        {
            list.Add(_serializer.Deserialize<T>(item));
        }

        return list;
    }

    public async Task<T> SPopAsync<T>(string cacheKey)
    {
        var bytes = await _redisDatabase.SetPopAsync(cacheKey);

        return _serializer.Deserialize<T>(bytes);
    }

    public async Task<List<T>> SRandMemberAsync<T>(string cacheKey, int count = 1)
    {
        var list = new List<T>();

        var bytes = await _redisDatabase.SetRandomMembersAsync(cacheKey, count);

        foreach (var item in bytes)
        {
            list.Add(_serializer.Deserialize<T>(item));
        }

        return list;
    }

    public async Task<long> SRemAsync<T>(string cacheKey, IEnumerable<T>? cacheValues = null)
    {
        if (cacheValues?.Any() ?? false)
        {
            var bytes = new List<RedisValue>();

            foreach (var item in cacheValues)
            {
                bytes.Add(_serializer.Serialize<T>(item));
            }

            return await _redisDatabase.SetRemoveAsync(cacheKey, bytes.ToArray());
        }

        var flag = await _redisDatabase.KeyDeleteAsync(cacheKey);
        return flag ? 1 : 0;
    }
}