using Tenon.BloomFilter.Abstractions;
using Tenon.Infra.Redis;

namespace Tenon.BloomFilter.Redis;

public sealed class RedisBloomFilter(IRedisProvider redisProvider) : IBloomFilter
{
    private readonly IRedisProvider _redisProvider =
        redisProvider ?? throw new ArgumentNullException(nameof(redisProvider));


    public async Task<bool> AddAsync(string key, string value)
    {
        return await _redisProvider.BfAddAsync(key, value);
    }

    public bool Add(string key, string value)
    {
        return _redisProvider.BfAdd(key, value);
    }

    public async Task<bool[]> AddAsync(string key, IEnumerable<string> values)
    {
        return await _redisProvider.BfAddAsync(key, values);
    }

    public bool[] Add(string key, IEnumerable<string> values)
    {
        return _redisProvider.BfAdd(key, values);
    }

    public async Task<bool> ExistsAsync(string key, string value)
    {
        return await _redisProvider.BfExistsAsync(key, value);
    }

    public bool Exists(string key, string value)
    {
        return _redisProvider.BfExists(key, value);
    }

    public async Task<bool[]> ExistsAsync(string key, IEnumerable<string> values)
    {
        return await _redisProvider.BfExistsAsync(key, values);
    }

    public async Task ReserveAsync(string key, double errorRate, int initialCapacity)
    {
        await _redisProvider.BfReserveAsync(key, errorRate, initialCapacity);
    }

    public void Reserve(string key, double errorRate, int initialCapacity)
    {
        _redisProvider.BfReserve(key, errorRate, initialCapacity);
    }
}