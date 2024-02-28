using System;
using System.Threading.Tasks;

namespace Tenon.Infra.Redis;

public interface IRedisStringProvider
{

    /// <summary>
    ///     https://redis.io/commands/incrby
    /// </summary>
    long IncrBy(string cacheKey, long value = 1);

    /// <summary>
    ///     https://redis.io/commands/incrby
    /// </summary>
    Task<long> IncrByAsync(string cacheKey, long value = 1);

    /// <summary>
    ///     https://redis.io/commands/incrbyfloat
    /// </summary>
    double IncrByFloat(string cacheKey, double value = 1);

    /// <summary>
    ///     https://redis.io/commands/incrbyfloat
    /// </summary>
    Task<double> IncrByFloatAsync(string cacheKey, double value = 1);

    /// <summary>
    ///     https://redis.io/commands/set
    /// </summary>
    bool StringSet(string cacheKey, string cacheValue, TimeSpan? expiration = null,
        StringSetWhen setWhen = StringSetWhen.Always);

    /// <summary>
    ///     https://redis.io/commands/set
    /// </summary>
    Task<bool> StringSetAsync(string cacheKey, string cacheValue, TimeSpan? expiration = null,
        StringSetWhen setWhen = StringSetWhen.Always);

    /// <summary>
    ///     https://redis.io/commands/get
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <returns></returns>
    string StringGet(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/get
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <returns></returns>
    Task<string> StringGetAsync(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/strlen
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <returns></returns>
    long StringLen(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/strlen
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <returns></returns>
    Task<long> StringLenAsync(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/setrange
    /// </summary>
    long StringSetRange(string cacheKey, long offset, string value);

    /// <summary>
    ///     https://redis.io/commands/setrange
    /// </summary>
    Task<long> StringSetRangeAsync(string cacheKey, long offset, string value);

    /// <summary>
    ///     https://redis.io/commands/getrange
    /// </summary>
    string StringGetRange(string cacheKey, long start, long end);

    /// <summary>
    ///     https://redis.io/commands/getrange
    /// </summary>
    Task<string> StringGetRangeAsync(string cacheKey, long start, long end);
}

public enum StringSetWhen
{
    /// <summary>
    ///     Always
    /// </summary>
    Always,

    /// <summary>
    ///     Only set the key if it does not already exist
    /// </summary>
    NotExists,

    /// <summary>
    ///     Only set the key if it already exists
    /// </summary>
    Exists
}