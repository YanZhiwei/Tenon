namespace Tenon.Redis.StackExchangeProvider;

public class StackExchangeProvider(RedisDatabase redisDatabase) : IRedisProvider
{
    private readonly RedisDatabase _redisDatabase = redisDatabase;

    public long IncrBy(string cacheKey, long value = 1)
    {
        throw new NotImplementedException();
    }

    public Task<long> IncrByAsync(string cacheKey, long value = 1)
    {
        throw new NotImplementedException();
    }

    public double IncrByFloat(string cacheKey, double value = 1)
    {
        throw new NotImplementedException();
    }

    public Task<double> IncrByFloatAsync(string cacheKey, double value = 1)
    {
        throw new NotImplementedException();
    }

    public bool StringSet(string cacheKey, string cacheValue, TimeSpan? expiration = null,
        StringSetWhen setWhen = StringSetWhen.Always)
    {
        throw new NotImplementedException();
    }

    public Task<bool> StringSetAsync(string cacheKey, string cacheValue, TimeSpan? expiration = null,
        StringSetWhen setWhen = StringSetWhen.Always)
    {
        throw new NotImplementedException();
    }

    public string StringGet(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public Task<string> StringGetAsync(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public long StringLen(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public Task<long> StringLenAsync(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public long StringSetRange(string cacheKey, long offset, string value)
    {
        throw new NotImplementedException();
    }

    public Task<long> StringSetRangeAsync(string cacheKey, long offset, string value)
    {
        throw new NotImplementedException();
    }

    public string StringGetRange(string cacheKey, long start, long end)
    {
        throw new NotImplementedException();
    }

    public Task<string> StringGetRangeAsync(string cacheKey, long start, long end)
    {
        throw new NotImplementedException();
    }

    public bool HMSet(string cacheKey, IDictionary<string, string> vals, TimeSpan? expiration = null)
    {
        throw new NotImplementedException();
    }

    public bool HSet(string cacheKey, string field, string cacheValue)
    {
        throw new NotImplementedException();
    }

    public bool HExists(string cacheKey, string field)
    {
        throw new NotImplementedException();
    }

    public long HDel(string cacheKey, IEnumerable<string> fields = null)
    {
        throw new NotImplementedException();
    }

    public string HGet(string cacheKey, string field)
    {
        throw new NotImplementedException();
    }

    public Dictionary<string, string> HGetAll(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public long HIncrBy(string cacheKey, string field, long val = 1)
    {
        throw new NotImplementedException();
    }

    public List<string> HKeys(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public long HLen(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public List<string> HVals(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public Dictionary<string, string> HMGet(string cacheKey, IEnumerable<string> fields)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HMSetAsync(string cacheKey, IDictionary<string, string> vals, TimeSpan? expiration = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HSetAsync(string cacheKey, string field, string cacheValue)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HExistsAsync(string cacheKey, string field)
    {
        throw new NotImplementedException();
    }

    public Task<long> HDelAsync(string cacheKey, IEnumerable<string> fields = null)
    {
        throw new NotImplementedException();
    }

    public Task<string> HGetAsync(string cacheKey, string field)
    {
        throw new NotImplementedException();
    }

    public Task<Dictionary<string, string>> HGetAllAsync(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public Task<long> HIncrByAsync(string cacheKey, string field, long val = 1)
    {
        throw new NotImplementedException();
    }

    public Task<List<string>> HKeysAsync(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public Task<long> HLenAsync(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public Task<List<string>> HValsAsync(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public Task<Dictionary<string, string>> HMGetAsync(string cacheKey, IEnumerable<string> fields)
    {
        throw new NotImplementedException();
    }

    public T LIndex<T>(string cacheKey, long index)
    {
        throw new NotImplementedException();
    }

    public long LLen(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public T LPop<T>(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public long LPush<T>(string cacheKey, IEnumerable<T> cacheValues)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<T> LRange<T>(string cacheKey, long start, long stop)
    {
        throw new NotImplementedException();
    }

    public long LRem<T>(string cacheKey, long count, T cacheValue)
    {
        throw new NotImplementedException();
    }

    public bool LSet<T>(string cacheKey, long index, T cacheValue)
    {
        throw new NotImplementedException();
    }

    public bool LTrim(string cacheKey, long start, long stop)
    {
        throw new NotImplementedException();
    }

    public long LPushX<T>(string cacheKey, T cacheValue)
    {
        throw new NotImplementedException();
    }

    public long LInsertBefore<T>(string cacheKey, T pivot, T cacheValue)
    {
        throw new NotImplementedException();
    }

    public long LInsertAfter<T>(string cacheKey, T pivot, T cacheValue)
    {
        throw new NotImplementedException();
    }

    public long RPushX<T>(string cacheKey, T cacheValue)
    {
        throw new NotImplementedException();
    }

    public long RPush<T>(string cacheKey, IEnumerable<T> cacheValues)
    {
        throw new NotImplementedException();
    }

    public T RPop<T>(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public Task<T> LIndexAsync<T>(string cacheKey, long index)
    {
        throw new NotImplementedException();
    }

    public Task<long> LLenAsync(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public Task<T> LPopAsync<T>(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public Task<long> LPushAsync<T>(string cacheKey, IEnumerable<T> cacheValues)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> LRangeAsync<T>(string cacheKey, long start, long stop)
    {
        throw new NotImplementedException();
    }

    public Task<long> LRemAsync<T>(string cacheKey, long count, T cacheValue)
    {
        throw new NotImplementedException();
    }

    public Task<bool> LSetAsync<T>(string cacheKey, long index, T cacheValue)
    {
        throw new NotImplementedException();
    }

    public Task<bool> LTrimAsync(string cacheKey, long start, long stop)
    {
        throw new NotImplementedException();
    }

    public Task<long> LPushXAsync<T>(string cacheKey, T cacheValue)
    {
        throw new NotImplementedException();
    }

    public Task<long> LInsertBeforeAsync<T>(string cacheKey, T pivot, T cacheValue)
    {
        throw new NotImplementedException();
    }

    public Task<long> LInsertAfterAsync<T>(string cacheKey, T pivot, T cacheValue)
    {
        throw new NotImplementedException();
    }

    public Task<long> RPushXAsync<T>(string cacheKey, T cacheValue)
    {
        throw new NotImplementedException();
    }

    public Task<long> RPushAsync<T>(string cacheKey, IEnumerable<T> cacheValues)
    {
        throw new NotImplementedException();
    }

    public Task<T> RPopAsync<T>(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public long ZAdd<T>(string cacheKey, IDictionary<T, double> cacheValues) where T : notnull
    {
        throw new NotImplementedException();
    }

    public long ZCard(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public long ZCount(string cacheKey, double min, double max)
    {
        throw new NotImplementedException();
    }

    public double ZIncrBy(string cacheKey, string field, double val = 1)
    {
        throw new NotImplementedException();
    }

    public long ZLexCount(string cacheKey, string min, string max)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<T> ZRange<T>(string cacheKey, long start, long stop) where T : notnull
    {
        throw new NotImplementedException();
    }

    public long? ZRank<T>(string cacheKey, T cacheValue) where T : notnull
    {
        throw new NotImplementedException();
    }

    public long ZRem<T>(string cacheKey, IEnumerable<T> cacheValues) where T : notnull
    {
        throw new NotImplementedException();
    }

    public double? ZScore<T>(string cacheKey, T cacheValue) where T : notnull
    {
        throw new NotImplementedException();
    }

    public Task<long> ZAddAsync<T>(string cacheKey, Dictionary<T, double> cacheValues) where T : notnull
    {
        throw new NotImplementedException();
    }

    public Task<long> ZCardAsync(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public Task<long> ZCountAsync(string cacheKey, double min, double max)
    {
        throw new NotImplementedException();
    }

    public Task<double> ZIncrByAsync(string cacheKey, string field, double val = 1)
    {
        throw new NotImplementedException();
    }

    public Task<long> ZLexCountAsync(string cacheKey, string min, string max)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> ZRangeAsync<T>(string cacheKey, long start, long stop)
    {
        throw new NotImplementedException();
    }

    public Task<long?> ZRankAsync<T>(string cacheKey, T cacheValue) where T : notnull
    {
        throw new NotImplementedException();
    }

    public Task<long> ZRemAsync<T>(string cacheKey, IEnumerable<T> cacheValues) where T : notnull
    {
        throw new NotImplementedException();
    }

    public Task<double?> ZScoreAsync<T>(string cacheKey, T cacheValue) where T : notnull
    {
        throw new NotImplementedException();
    }

    public bool KeyDel(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public Task<bool> KeyDelAsync(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public bool KeyExpire(string cacheKey, int second)
    {
        throw new NotImplementedException();
    }

    public Task<bool> KeyExpireAsync(string cacheKey, int second)
    {
        throw new NotImplementedException();
    }

    public Task<bool> KeyExistsAsync(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public bool KeyExists(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public long TTL(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public Task<long> TTLAsync(string cacheKey)
    {
        throw new NotImplementedException();
    }
}