using Tenon.Redis;

namespace Tenon.BloomFilter.Redis;

public sealed class RedisBloomFilter(IRedisProvider redisProvider) : IBloomFilter
{
    private readonly IRedisProvider _redisProvider =
        redisProvider ?? throw new ArgumentNullException(nameof(redisProvider));


    public async Task<bool> AddAsync(string key, string value)
    {
        return await _redisProvider.BfAddAsync(key, value);
    }

    public async Task<bool[]> AddAsync(string key, IEnumerable<string> values)
    {
        return await _redisProvider.BfAddAsync(key, values);
    }

    public async Task<bool> ExistsAsync(string key, string value)
    {
        return await _redisProvider.BfExistsAsync(key, value);
    }

    public async Task<bool[]> ExistsAsync(string key, IEnumerable<string> values)
    {
        return await _redisProvider.BfExistsAsync(key, values);
    }

    public async Task ReserveAsync(string key, double errorRate, int initialCapacity)
    {
        await _redisProvider.BfReserveAsync(key, errorRate, initialCapacity);
    }
}