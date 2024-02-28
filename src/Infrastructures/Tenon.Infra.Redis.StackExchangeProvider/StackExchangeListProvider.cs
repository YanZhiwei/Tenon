using StackExchange.Redis;

namespace Tenon.Infra.Redis.StackExchangeProvider;

public partial class StackExchangeProvider
{
    public T LIndex<T>(string cacheKey, long index)
    {
        var bytes = _redisDatabase.ListGetByIndex(cacheKey, index);
        return _serializer.Deserialize<T>(bytes);
    }

    public long LLen(string cacheKey)
    {
        return _redisDatabase.ListLength(cacheKey);
    }

    public T LPop<T>(string cacheKey)
    {
        var bytes = _redisDatabase.ListLeftPop(cacheKey);
        return _serializer.Deserialize<T>(bytes);
    }

    public long LPush<T>(string cacheKey, IEnumerable<T> cacheValues)
    {
        var list = new List<RedisValue>();
        foreach (var item in cacheValues) list.Add(_serializer.Serialize(item));

        return _redisDatabase.ListLeftPush(cacheKey, list.ToArray());
    }

    public IEnumerable<T> LRange<T>(string cacheKey, long start, long stop)
    {
        var list = new List<T>();

        var bytes = _redisDatabase.ListRange(cacheKey, start, stop);

        foreach (var item in bytes) list.Add(_serializer.Deserialize<T>(item));

        return list;
    }

    public long LRem<T>(string cacheKey, long count, T cacheValue)
    {
        var bytes = _serializer.Serialize(cacheValue);
        return _redisDatabase.ListRemove(cacheKey, bytes, count);
    }

    public bool LSet<T>(string cacheKey, long index, T cacheValue)
    {
        var bytes = _serializer.Serialize(cacheValue);
        _redisDatabase.ListSetByIndex(cacheKey, index, bytes);
        return true;
    }

    public bool LTrim(string cacheKey, long start, long stop)
    {
        _redisDatabase.ListTrim(cacheKey, start, stop);
        return true;
    }

    public long LPushX<T>(string cacheKey, T cacheValue)
    {
        var bytes = _serializer.Serialize(cacheValue);
        return _redisDatabase.ListLeftPush(cacheKey, bytes);
    }

    public long LInsertBefore<T>(string cacheKey, T pivot, T cacheValue)
    {
        var pivotBytes = _serializer.Serialize(pivot);
        var cacheValueBytes = _serializer.Serialize(cacheValue);
        return _redisDatabase.ListInsertBefore(cacheKey, pivotBytes, cacheValueBytes);
    }

    public long LInsertAfter<T>(string cacheKey, T pivot, T cacheValue)
    {
        var pivotBytes = _serializer.Serialize(pivot);
        var cacheValueBytes = _serializer.Serialize(cacheValue);
        return _redisDatabase.ListInsertAfter(cacheKey, pivotBytes, cacheValueBytes);
    }

    public long RPushX<T>(string cacheKey, T cacheValue)
    {
        var bytes = _serializer.Serialize(cacheValue);
        return _redisDatabase.ListRightPush(cacheKey, bytes);
    }

    public long RPush<T>(string cacheKey, IEnumerable<T> cacheValues)
    {
        var list = new List<RedisValue>();

        foreach (var item in cacheValues) list.Add(_serializer.Serialize(item));

        return _redisDatabase.ListRightPush(cacheKey, list.ToArray());
    }

    public T RPop<T>(string cacheKey)
    {
        var bytes = _redisDatabase.ListRightPop(cacheKey);
        return _serializer.Deserialize<T>(bytes);
    }

    public async Task<T> LIndexAsync<T>(string cacheKey, long index)
    {
        var bytes = await _redisDatabase.ListGetByIndexAsync(cacheKey, index);
        return _serializer.Deserialize<T>(bytes);
    }

    public async Task<long> LLenAsync(string cacheKey)
    {
        return await _redisDatabase.ListLengthAsync(cacheKey);
    }

    public async Task<T> LPopAsync<T>(string cacheKey)
    {
        var bytes = await _redisDatabase.ListLeftPopAsync(cacheKey);
        return _serializer.Deserialize<T>(bytes);
    }

    public async Task<long> LPushAsync<T>(string cacheKey, IEnumerable<T> cacheValues)
    {
        var list = new List<RedisValue>();

        foreach (var item in cacheValues) list.Add(_serializer.Serialize(item));

        return await _redisDatabase.ListLeftPushAsync(cacheKey, list.ToArray());
    }

    public async Task<IEnumerable<T>> LRangeAsync<T>(string cacheKey, long start, long stop)
    {
        var list = new List<T>();

        var bytes = await _redisDatabase.ListRangeAsync(cacheKey, start, stop);

        foreach (var item in bytes) list.Add(_serializer.Deserialize<T>(item));

        return list;
    }

    public async Task<long> LRemAsync<T>(string cacheKey, long count, T cacheValue)
    {
        var bytes = _serializer.Serialize(cacheValue);
        return await _redisDatabase.ListRemoveAsync(cacheKey, bytes, count);
    }

    public async Task<bool> LSetAsync<T>(string cacheKey, long index, T cacheValue)
    {
        var bytes = _serializer.Serialize(cacheValue);
        await _redisDatabase.ListSetByIndexAsync(cacheKey, index, bytes);
        return true;
    }

    public async Task<bool> LTrimAsync(string cacheKey, long start, long stop)
    {
        await _redisDatabase.ListTrimAsync(cacheKey, start, stop);
        return true;
    }

    public async Task<long> LPushXAsync<T>(string cacheKey, T cacheValue)
    {
        var bytes = _serializer.Serialize(cacheValue);
        return await _redisDatabase.ListLeftPushAsync(cacheKey, bytes);
    }

    public async Task<long> LInsertBeforeAsync<T>(string cacheKey, T pivot, T cacheValue)
    {
        var pivotBytes = _serializer.Serialize(pivot);
        var cacheValueBytes = _serializer.Serialize(cacheValue);
        return await _redisDatabase.ListInsertBeforeAsync(cacheKey, pivotBytes, cacheValueBytes);
    }

    public async Task<long> LInsertAfterAsync<T>(string cacheKey, T pivot, T cacheValue)
    {
        var pivotBytes = _serializer.Serialize(pivot);
        var cacheValueBytes = _serializer.Serialize(cacheValue);
        return await _redisDatabase.ListInsertAfterAsync(cacheKey, pivotBytes, cacheValueBytes);
    }

    public async Task<long> RPushXAsync<T>(string cacheKey, T cacheValue)
    {
        var bytes = _serializer.Serialize(cacheValue);
        return await _redisDatabase.ListRightPushAsync(cacheKey, bytes);
    }

    public async Task<long> RPushAsync<T>(string cacheKey, IEnumerable<T> cacheValues)
    {
        var list = new List<RedisValue>();

        foreach (var item in cacheValues) list.Add(_serializer.Serialize(item));

        return await _redisDatabase.ListRightPushAsync(cacheKey, list.ToArray());
    }

    public async Task<T> RPopAsync<T>(string cacheKey)
    {
        var bytes = await _redisDatabase.ListRightPopAsync(cacheKey);
        return _serializer.Deserialize<T>(bytes);
    }
}