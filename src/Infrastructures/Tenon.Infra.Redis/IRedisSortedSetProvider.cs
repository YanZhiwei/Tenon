using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tenon.Infra.Redis;

public interface IRedisSortedSetProvider
{
    /// <summary>
    ///     https://redis.io/commands/zadd
    /// </summary>
    long ZAdd<T>(string cacheKey, IDictionary<T, double> cacheValues) where T : notnull;

    /// <summary>
    ///     https://redis.io/commands/zcard
    /// </summary>
    long ZCard(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/zcount
    /// </summary>
    long ZCount(string cacheKey, double min, double max);

    /// <summary>
    ///     https://redis.io/commands/zincrby
    /// </summary>
    double ZIncrBy(string cacheKey, string field, double val = 1);

    /// <summary>
    ///     https://redis.io/commands/zlexcount
    /// </summary>
    long ZLexCount(string cacheKey, string min, string max);

    /// <summary>
    ///     https://redis.io/commands/zrange
    /// </summary>
    IEnumerable<T> ZRange<T>(string cacheKey, long start, long stop) where T : notnull;

    /// <summary>
    ///     https://redis.io/commands/zrank
    /// </summary>
    long? ZRank<T>(string cacheKey, T cacheValue) where T : notnull;

    /// <summary>
    ///     https://redis.io/commands/zrem
    /// </summary>
    long ZRem<T>(string cacheKey, IEnumerable<T> cacheValues) where T : notnull;

    /// <summary>
    ///     https://redis.io/commands/zscore
    /// </summary>
    double? ZScore<T>(string cacheKey, T cacheValue) where T : notnull;

    /// <summary>
    ///     https://redis.io/commands/zadd
    /// </summary>
    Task<long> ZAddAsync<T>(string cacheKey, Dictionary<T, double> cacheValues) where T : notnull;

    /// <summary>
    ///     https://redis.io/commands/zcard
    /// </summary>
    Task<long> ZCardAsync(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/zcount
    /// </summary>
    Task<long> ZCountAsync(string cacheKey, double min, double max);

    /// <summary>
    ///     https://redis.io/commands/zincrby
    /// </summary>
    Task<double> ZIncrByAsync(string cacheKey, string field, double val = 1);

    /// <summary>
    ///     https://redis.io/commands/zlexcount
    /// </summary>
    Task<long> ZLexCountAsync(string cacheKey, string min, string max);

    /// <summary>
    ///     https://redis.io/commands/zrange
    /// </summary>
    Task<IEnumerable<T>> ZRangeAsync<T>(string cacheKey, long start, long stop);

    /// <summary>
    ///     https://redis.io/commands/zrank
    /// </summary>
    Task<long?> ZRankAsync<T>(string cacheKey, T cacheValue) where T : notnull;

    /// <summary>
    ///     https://redis.io/commands/zrem
    /// </summary>
    Task<long> ZRemAsync<T>(string cacheKey, IEnumerable<T> cacheValues) where T : notnull;

    /// <summary>
    ///     https://redis.io/commands/zscore
    /// </summary>
    Task<double?> ZScoreAsync<T>(string cacheKey, T cacheValue) where T : notnull;
}