using StackExchange.Redis;

namespace Tenon.StackExchange.Redis.Extensions;

// ReSharper disable once InconsistentNaming
public static class IDatabaseExtension
{
    /// <summary>
    ///     https://redis.io/commands/bf.add/
    /// </summary>
    public static async Task<bool> BfAddAsync(this IDatabase redisDb, RedisKey key, RedisValue value)
    {
        return (bool)await redisDb.ExecuteAsync("BF.ADD", key, value);
    }

    /// <summary>
    ///     https://redis.io/commands/bf.exists/
    /// </summary>
    public static async Task<bool> BfExistsAsync(this IDatabase redisDb, RedisKey key, RedisValue value)
    {
        return (bool)await redisDb.ExecuteAsync("BF.EXISTS", key, value);
    }

    /// <summary>
    ///     https://redis.io/commands/bf.madd/
    /// </summary>
    public static async Task<bool[]> BfMAddAsync(this IDatabase redisDb, RedisKey key, RedisValue[] values)
    {
        return (bool[]?)await redisDb.ExecuteAsync("BF.MADD", values.Cast<object>().Prepend(key).ToArray()) ??
               Array.Empty<bool>();
    }

    /// <summary>
    ///     https://redis.io/commands/bf.mexists/
    /// </summary>
    public static async Task<bool> BfMExistsAsync(this IDatabase redisDb, RedisKey key, RedisValue[] values)
    {
        return (bool)await redisDb.ExecuteAsync("BF.MEXISTS", key, values);
    }

    /// <summary>
    ///     https://redis.io/commands/bf.info/
    /// </summary>
    public static async Task<long> BfInfoAsync(this IDatabase redisDb, RedisKey key)
    {
        return (long)await redisDb.ExecuteAsync("BF.INFO", key);
    }

    /// <summary>
    ///     https://redis.io/commands/bf.mexists/
    /// </summary>
    public static async Task<bool[]> BfExistsAsync(this IDatabase redisDb, RedisKey key, IEnumerable<RedisValue> values)
    {
        return (bool[]?)await redisDb.ExecuteAsync("BF.MEXISTS", values.Cast<object>().Prepend(key).ToArray()) ??
               Array.Empty<bool>();
    }


    /// <summary>
    ///     https://redis.io/commands/bf.reserve/
    /// </summary>
    public static async Task BfReserveAsync(this IDatabase redisDb, RedisKey key, double errorRate,
        int initialCapacity)
    {
        await redisDb.ExecuteAsync("BF.RESERVE", key, errorRate, initialCapacity);
    }

    public static async Task<(bool Success, string LockValue)> LockAsync(this IDatabase redisDb, string cacheKey, int timeoutSeconds = 5, bool autoDelay = false)
    {
       
       
    }
}