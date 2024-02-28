using StackExchange.Redis;

namespace Tenon.Infra.Redis.StackExchangeProvider;

public partial class StackExchangeProvider : IRedisProvider
{
    public long ZAdd<T>(string cacheKey, IDictionary<T, double> cacheValues) where T : notnull
    {
        var param = new List<SortedSetEntry>();

        foreach (var item in cacheValues)
            param.Add(new SortedSetEntry(_serializer.Serialize(item.Key), item.Value));

        return _redisDatabase.SortedSetAdd(cacheKey, param.ToArray());
    }

    public long ZCard(string cacheKey)
    {
        return _redisDatabase.SortedSetLength(cacheKey);
    }

    public long ZCount(string cacheKey, double min, double max)
    {
        return _redisDatabase.SortedSetLengthByValue(cacheKey, min, max);
    }

    public double ZIncrBy(string cacheKey, string field, double val = 1)
    {
        return _redisDatabase.SortedSetIncrement(cacheKey, field, val);
    }

    public long ZLexCount(string cacheKey, string min, string max)
    {
        return _redisDatabase.SortedSetLengthByValue(cacheKey, min, max);
    }

    public IEnumerable<T> ZRange<T>(string cacheKey, long start, long stop) where T : notnull
    {
        var list = new List<T>();

        var bytes = _redisDatabase.SortedSetRangeByRank(cacheKey, start, stop);

        foreach (var item in bytes) list.Add(_serializer.Deserialize<T>(item));

        return list;
    }

    public long? ZRank<T>(string cacheKey, T cacheValue) where T : notnull
    {
        var bytes = _serializer.Serialize(cacheValue);
        return _redisDatabase.SortedSetRank(cacheKey, bytes);
    }

    public long ZRem<T>(string cacheKey, IEnumerable<T> cacheValues) where T : notnull
    {
        var bytes = new List<RedisValue>();
        foreach (var item in cacheValues) bytes.Add(_serializer.Serialize(item));
        return _redisDatabase.SortedSetRemove(cacheKey, bytes.ToArray());
    }

    public double? ZScore<T>(string cacheKey, T cacheValue) where T : notnull
    {
        var bytes = _serializer.Serialize(cacheValue);
        return _redisDatabase.SortedSetScore(cacheKey, bytes);
    }

    public async Task<long> ZAddAsync<T>(string cacheKey, Dictionary<T, double> cacheValues) where T : notnull
    {
        var list = new List<SortedSetEntry>();

        foreach (var item in cacheValues) list.Add(new SortedSetEntry(_serializer.Serialize(item.Key), item.Value));

        return await _redisDatabase.SortedSetAddAsync(cacheKey, list.ToArray());
    }

    public async Task<long> ZCardAsync(string cacheKey)
    {
        return await _redisDatabase.SortedSetLengthAsync(cacheKey);
    }

    public async Task<long> ZCountAsync(string cacheKey, double min, double max)
    {
        return await _redisDatabase.SortedSetLengthByValueAsync(cacheKey, min, max);
    }

    public async Task<double> ZIncrByAsync(string cacheKey, string field, double val = 1)
    {
        return await _redisDatabase.SortedSetIncrementAsync(cacheKey, field, val);
    }

    public async Task<long> ZLexCountAsync(string cacheKey, string min, string max)
    {
        return await _redisDatabase.SortedSetLengthByValueAsync(cacheKey, min, max);
    }

    public async Task<IEnumerable<T>> ZRangeAsync<T>(string cacheKey, long start, long stop)
    {
        throw new NotImplementedException();
    }

    public async Task<long?> ZRankAsync<T>(string cacheKey, T cacheValue) where T : notnull
    {
        var bytes = _serializer.Serialize(cacheValue);
        return await _redisDatabase.SortedSetRankAsync(cacheKey, bytes);
    }

    public async Task<long> ZRemAsync<T>(string cacheKey, IEnumerable<T> cacheValues) where T : notnull
    {
        var bytes = new List<RedisValue>();

        foreach (var item in cacheValues) bytes.Add(_serializer.Serialize(item));
        return await _redisDatabase.SortedSetRemoveAsync(cacheKey, bytes.ToArray());
    }

    public async Task<double?> ZScoreAsync<T>(string cacheKey, T cacheValue) where T : notnull
    {
        var bytes = _serializer.Serialize(cacheValue);
        return await _redisDatabase.SortedSetScoreAsync(cacheKey, bytes);
    }
}