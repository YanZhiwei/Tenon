using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tenon.Redis;

public interface IRedisKeyProvider
{
    /// <summary>
    ///     https://redis.io/commands/del
    /// </summary>
    bool KeyDelete(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/del
    /// </summary>
    long KeyDelete(IEnumerable<string> cacheKeys);

    /// <summary>
    ///     https://redis.io/commands/del
    /// </summary>
    Task<bool> KeyDeleteAsync(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/del
    /// </summary>
    Task<long> KeysDeleteAsync(IEnumerable<string> cacheKeys);

    /// <summary>
    ///     https://redis.io/commands/expire
    /// </summary>
    bool KeyExpire(string cacheKey, int second);

    /// <summary>
    ///     https://redis.io/commands/expire
    /// </summary>
    Task<bool> KeyExpireAsync(string cacheKey, int second);

    /// <summary>
    ///     https://redis.io/commands/expire
    /// </summary>
    Task<long> KeysExpireAsync(IEnumerable<string> cacheKeys);

    /// <summary>
    ///     https://redis.io/commands/expire
    /// </summary>
    Task<bool> KeyExistsAsync(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/expire
    /// </summary>
    /// <returns></returns>
    bool KeyExists(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/ttl
    /// </summary>
    long TTL(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/ttl
    /// </summary>
    Task<long> TTLAsync(string cacheKey);
}