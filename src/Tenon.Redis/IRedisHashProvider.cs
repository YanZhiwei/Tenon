using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tenon.Redis;

public interface IRedisHashProvider
{
    /// <summary>
    ///     https://redis.io/commands/hmset
    /// </summary>
    bool HMSet(string cacheKey, IDictionary<string, string> vals, TimeSpan? expiration = null);

    /// <summary>
    ///     https://redis.io/commands/hset
    /// </summary>
    bool HSet(string cacheKey, string field, string cacheValue);

    /// <summary>
    ///     https://redis.io/commands/hexists
    /// </summary>
    bool HExists(string cacheKey, string field);

    /// <summary>
    ///     https://redis.io/commands/hdel
    /// </summary>
    long HDel(string cacheKey, IEnumerable<string> fields = null);

    /// <summary>
    ///     https://redis.io/commands/hget
    /// </summary>
    string HGet(string cacheKey, string field);

    /// <summary>
    ///     https://redis.io/commands/hgetall
    /// </summary>
    IDictionary<string, string> HGetAll(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/hincrby
    /// </summary>
    long HIncrBy(string cacheKey, string field, long val = 1);

    /// <summary>
    ///     https://redis.io/commands/hkeys
    /// </summary>
    IEnumerable<string> HKeys(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/hlen
    /// </summary>
    long HLen(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/hvals
    /// </summary>
    IEnumerable<string> HVals(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/hmget
    /// </summary>
    IDictionary<string, string> HMGet(string cacheKey, IEnumerable<string> fields);

    /// <summary>
    ///     https://redis.io/commands/hset
    /// </summary>
    Task<bool> HMSetAsync(string cacheKey, IDictionary<string, string> vals, TimeSpan? expiration = null);

    /// <summary>
    ///     https://redis.io/commands/hset
    /// </summary>
    Task<bool> HSetAsync(string cacheKey, string field, string cacheValue);

    /// <summary>
    ///     https://redis.io/commands/hexists
    /// </summary>
    Task<bool> HExistsAsync(string cacheKey, string field);

    /// <summary>
    ///     https://redis.io/commands/hdel
    /// </summary>
    Task<long> HDelAsync(string cacheKey, IEnumerable<string> fields = null);

    /// <summary>
    ///     https://redis.io/commands/hget
    /// </summary>
    Task<string> HGetAsync(string cacheKey, string field);

    /// <summary>
    ///     https://redis.io/commands/hgetall
    /// </summary>
    Task<IDictionary<string, string>> HGetAllAsync(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/hincrby
    /// </summary>
    Task<long> HIncrByAsync(string cacheKey, string field, long val = 1);

    /// <summary>
    ///     https://redis.io/commands/hkeys
    /// </summary>
    Task<IEnumerable<string>> HKeysAsync(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/hlen
    /// </summary>
    Task<long> HLenAsync(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/hvals
    /// </summary>
    Task<IEnumerable<string>> HValsAsync(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/hmget
    /// </summary>
    Task<IDictionary<string, string>> HMGetAsync(string cacheKey, IEnumerable<string> fields);
}