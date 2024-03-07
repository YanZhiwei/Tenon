using Microsoft.Extensions.Options;
using Tenon.BloomFilter.Abstractions;
using Tenon.BloomFilter.Abstractions.Configurations;
using Tenon.Infra.Redis;

namespace Tenon.BloomFilter.Redis;

public sealed class RedisBloomFilter(IRedisProvider redisProvider, BloomFilterOptions options)
    : IBloomFilter
{
    private readonly string _key = options?.Name ?? throw new ArgumentNullException(nameof(Options.Name));

    private readonly IRedisProvider _redisProvider =
        redisProvider ?? throw new ArgumentNullException(nameof(redisProvider));

    public BloomFilterOptions Options => options ?? throw new ArgumentNullException(nameof(options));

    public async Task InitAsync()
    {
        if (await ExistsAsync())
            return;

        await _redisProvider.BfReserveAsync(_key, Options.ErrorRate, Options.Capacity);
    }

    public void Init()
    {
        if (Exists())
            return;

        _redisProvider.BfReserve(_key, Options.ErrorRate, Options.Capacity);
    }

    public async Task<bool> AddAsync(string value)
    {
        await CheckInitAsync();
        return await _redisProvider.BfAddAsync(_key, value);
    }

    public bool Add(string value)
    {
        CheckInit();
        return _redisProvider.BfAdd(_key, value);
    }

    public async Task<bool[]> AddAsync(IEnumerable<string> values)
    {
        await CheckInitAsync();
        return await _redisProvider.BfAddAsync(_key, values);
    }

    public bool[] Add(IEnumerable<string> values)
    {
        CheckInit();
        return _redisProvider.BfAdd(_key, values);
    }

    public async Task<bool> ExistsAsync(string value)
    {
        return await _redisProvider.BfExistsAsync(_key, value);
    }

    public bool Exists(string value)
    {
        return _redisProvider.BfExists(_key, value);
    }

    public async Task<bool[]> ExistsAsync(IEnumerable<string> values)
    {
        return await _redisProvider.BfExistsAsync(_key, values);
    }

    public async Task<bool> ExistsAsync()
    {
        return await _redisProvider.KeyExistsAsync(_key);
    }

    public bool Exists()
    {
        return _redisProvider.KeyExists(_key);
    }

    private async Task CheckInitAsync()
    {
        var exists = await ExistsAsync();
        if (!exists)
            throw new InvalidOperationException($"BloomFilter:{Options.Name} not init");
    }

    private void CheckInit()
    {
        var exists = Exists();
        if (!exists)
            throw new InvalidOperationException($"BloomFilter:{Options.Name} not init");
    }
}