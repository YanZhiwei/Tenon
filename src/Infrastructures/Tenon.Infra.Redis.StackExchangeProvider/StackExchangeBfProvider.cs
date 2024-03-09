using StackExchange.Redis;
using Tenon.StackExchange.Redis.Extensions;

namespace Tenon.Infra.Redis.StackExchangeProvider;

public partial class StackExchangeProvider
{
    public async Task<bool> BfAddAsync(string key, string value)
    {
        return await _redisDatabase.BfAddAsync(key, value);
    }

    public bool BfAdd(string key, string value)
    {
        return _redisDatabase.BfAdd(key, value);
    }

    public async Task<bool[]> BfAddAsync(string key, IEnumerable<string> values)
    {
        var redisValues = values.Select(x => (RedisValue)x).ToArray();
        return await _redisDatabase.BfMAddAsync(key, redisValues);
    }

    public bool[] BfAdd(string key, IEnumerable<string> values)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> BfExistsAsync(string key, string value)
    {
        return await _redisDatabase.BfExistsAsync(key, value);
    }

    public bool BfExists(string key, string value)
    {
        throw new NotImplementedException();
    }

    Task<dynamic> IRedisBfProvider.BfInfoAsync(string key)
    {
        throw new NotImplementedException();
    }

    public async Task<bool[]> BfExistsAsync(string key, IEnumerable<string> values)
    {
        var redisValues = values.Select(x => (RedisValue)x);
        return await _redisDatabase.BfExistsAsync(key, redisValues);
    }

    public bool[] BfExists(string key, IEnumerable<string> values)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> BfReserveAsync(string key, double errorRate, int initialCapacity)
    {
        return await _redisDatabase.BfReserveAsync(key, errorRate, initialCapacity);
    }

    public bool BfReserve(string key, double errorRate, int initialCapacity)
    {
        return _redisDatabase.BfReserve(key, errorRate, initialCapacity);
    }

    public async Task<long> BfInfoAsync(string key)
    {
        return await _redisDatabase.BfInfoAsync(key);
    }
}