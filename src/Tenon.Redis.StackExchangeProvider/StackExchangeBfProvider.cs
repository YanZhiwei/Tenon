using StackExchange.Redis;
using Tenon.StackExchange.Redis.Extensions;

namespace Tenon.Redis.StackExchangeProvider;

public partial class StackExchangeProvider
{
    public async Task<bool> BfAddAsync(string key, string value)
    {
        return await _redisDatabase.BfAddAsync(key, value);
    }

    public async Task<bool[]> BfAddAsync(string key, IEnumerable<string> values)
    {
        var redisValues = values.Select(x => (RedisValue)x).ToArray();
        return await _redisDatabase.BfMAddAsync(key, redisValues);
    }

    public async Task<bool> BfExistsAsync(string key, string value)
    {
        return await _redisDatabase.BfExistsAsync(key, value);
    }

    public async Task<long> BfInfoAsync(string key)
    {
        return await _redisDatabase.BfInfoAsync(key);
    }

    public async Task<bool[]> BfExistsAsync(string key, IEnumerable<string> values)
    {
        var redisValues = values.Select(x => (RedisValue)x);
        return await _redisDatabase.BfExistsAsync(key, redisValues);
    }

    public async Task BfReserveAsync(string key, double errorRate, int initialCapacity)
    {
        await _redisDatabase.BfReserveAsync(key, errorRate, initialCapacity);
    }
}