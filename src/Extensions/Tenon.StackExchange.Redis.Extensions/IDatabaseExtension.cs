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
    ///     https://redis.io/commands/bf.add/
    /// </summary>
    public static bool BfAdd(this IDatabase redisDb, RedisKey key, RedisValue value)
    {
        return (bool)redisDb.Execute("BF.ADD", key, value);
    }

    /// <summary>
    ///     https://redis.io/commands/bf.exists/
    /// </summary>
    public static async Task<bool> BfExistsAsync(this IDatabase redisDb, RedisKey key, RedisValue value)
    {
        return (bool)await redisDb.ExecuteAsync("BF.EXISTS", key, value);
    }

    /// <summary>
    ///     https://redis.io/commands/bf.exists/
    /// </summary>
    public static bool BfExists(this IDatabase redisDb, RedisKey key, RedisValue value)
    {
        return (bool)redisDb.Execute("BF.EXISTS", key, value);
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
    ///     https://redis.io/commands/bf.madd/
    /// </summary>
    public static bool[] BfMAdd(this IDatabase redisDb, RedisKey key, RedisValue[] values)
    {
        return (bool[]?)redisDb.Execute("BF.MADD", values.Cast<object>().Prepend(key).ToArray()) ??
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
    ///     https://redis.io/commands/bf.mexists/
    /// </summary>
    public static bool BfMExists(this IDatabase redisDb, RedisKey key, RedisValue[] values)
    {
        return (bool)redisDb.Execute("BF.MEXISTS", key, values);
    }

    /// <summary>
    ///     https://redis.io/commands/bf.info/
    /// </summary>
    public static async Task<dynamic> BfInfoAsync(this IDatabase redisDb, RedisKey key)
    {
        return await redisDb.ExecuteAsync("BF.INFO", key);
    }

    /// <summary>
    ///     https://redis.io/commands/bf.info/
    /// </summary>
    public static dynamic BfInfo(this IDatabase redisDb, RedisKey key)
    {
        return redisDb.Execute("BF.INFO", key);
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
    ///     https://redis.io/commands/bf.mexists/
    /// </summary>
    public static bool[] BfExists(this IDatabase redisDb, RedisKey key, IEnumerable<RedisValue> values)
    {
        return (bool[]?)redisDb.Execute("BF.MEXISTS", values.Cast<object>().Prepend(key).ToArray()) ??
               Array.Empty<bool>();
    }


    /// <summary>
    ///     https://redis.io/commands/bf.reserve/
    /// </summary>
    public static async Task<bool> BfReserveAsync(this IDatabase redisDb, RedisKey key, double errorRate,
        int initialCapacity)
    {
        var result= await redisDb.ExecuteAsync("BF.RESERVE", key, errorRate, initialCapacity);
        return !result.IsNull && !result.ToString().StartsWith("ERR", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     https://redis.io/commands/bf.reserve/
    /// </summary>
    public static bool BfReserve(this IDatabase redisDb, RedisKey key, double errorRate,
        int initialCapacity)
    {
        var result= redisDb.Execute("BF.RESERVE", key, errorRate, initialCapacity);
        return !result.IsNull && !result.ToString().StartsWith("ERR", StringComparison.OrdinalIgnoreCase);
    }
}