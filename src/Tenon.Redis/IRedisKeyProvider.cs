using System.Threading.Tasks;

namespace Tenon.Redis;

public interface IRedisKeyProvider
{
    /// <summary>
    ///     https://redis.io/commands/del
    /// </summary>
    bool KeyDel(string cacheKey);

    /// <summary>
    ///     https://redis.io/commands/del
    /// </summary>
    Task<bool> KeyDelAsync(string cacheKey);

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