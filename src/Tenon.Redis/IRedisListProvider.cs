using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tenon.Redis;

public interface IRedisListProvider
{
    /// <summary>
    ///     https://redis.io/commands/lindex
    /// </summary>
    T LIndex<T>(string cacheKey, long index);

    /// <summary>
    ///     https://redis.io/commands/llen
    /// </summary>
    long LLen(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/lpop
    /// </summary>
    T LPop<T>(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/lpush
    /// </summary>
    long LPush<T>(string cacheKey, IEnumerable<T> cacheValues);

    /// <summary>
    ///     https://redis.io/commands/lrange
    /// </summary>
    IEnumerable<T> LRange<T>(string cacheKey, long start, long stop);

    /// <summary>
    ///     https://redis.io/commands/lrem
    /// </summary>
    long LRem<T>(string cacheKey, long count, T cacheValue);

    /// <summary>
    ///     https://redis.io/commands/lset
    /// </summary>
    bool LSet<T>(string cacheKey, long index, T cacheValue);

    /// <summary>
    ///     https://redis.io/commands/ltrim
    /// </summary>
    bool LTrim(string cacheKey, long start, long stop);

    /// <summary>
    ///     https://redis.io/commands/lpushx
    /// </summary>
    long LPushX<T>(string cacheKey, T cacheValue);

    /// <summary>
    ///     https://redis.io/commands/linsert
    /// </summary>
    long LInsertBefore<T>(string cacheKey, T pivot, T cacheValue);

    /// <summary>
    ///     https://redis.io/commands/linsert
    /// </summary>
    long LInsertAfter<T>(string cacheKey, T pivot, T cacheValue);

    /// <summary>
    ///     https://redis.io/commands/rpushx
    /// </summary>
    long RPushX<T>(string cacheKey, T cacheValue);

    /// <summary>
    ///     https://redis.io/commands/rpush
    /// </summary>
    long RPush<T>(string cacheKey, IEnumerable<T> cacheValues);

    /// <summary>
    ///     https://redis.io/commands/rpop
    /// </summary>
    T RPop<T>(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/lindex
    /// </summary>
    Task<T> LIndexAsync<T>(string cacheKey, long index);

    /// <summary>
    ///     https://redis.io/commands/llen
    /// </summary>
    Task<long> LLenAsync(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/lpop
    /// </summary>
    Task<T> LPopAsync<T>(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/lpush
    /// </summary>
    Task<long> LPushAsync<T>(string cacheKey, IEnumerable<T> cacheValues);

    /// <summary>
    ///     https://redis.io/commands/lrange
    /// </summary>
    Task<IEnumerable<T>> LRangeAsync<T>(string cacheKey, long start, long stop);

    /// <summary>
    ///     https://redis.io/commands/lrem
    /// </summary>
    Task<long> LRemAsync<T>(string cacheKey, long count, T cacheValue);

    /// <summary>
    ///     https://redis.io/commands/lset
    /// </summary>
    Task<bool> LSetAsync<T>(string cacheKey, long index, T cacheValue);

    /// <summary>
    ///     https://redis.io/commands/ltrim
    /// </summary>
    Task<bool> LTrimAsync(string cacheKey, long start, long stop);

    /// <summary>
    ///     https://redis.io/commands/lpushx
    /// </summary>
    Task<long> LPushXAsync<T>(string cacheKey, T cacheValue);

    /// <summary>
    ///     https://redis.io/commands/linsert
    /// </summary>
    Task<long> LInsertBeforeAsync<T>(string cacheKey, T pivot, T cacheValue);

    /// <summary>
    ///     https://redis.io/commands/linsert
    /// </summary>
    Task<long> LInsertAfterAsync<T>(string cacheKey, T pivot, T cacheValue);

    /// <summary>
    ///     https://redis.io/commands/rpushx
    /// </summary>
    Task<long> RPushXAsync<T>(string cacheKey, T cacheValue);

    /// <summary>
    ///     https://redis.io/commands/rpush
    /// </summary>
    Task<long> RPushAsync<T>(string cacheKey, IEnumerable<T> cacheValues);

    /// <summary>
    ///     https://redis.io/commands/rpop
    /// </summary>
    Task<T> RPopAsync<T>(string cacheKey);
}