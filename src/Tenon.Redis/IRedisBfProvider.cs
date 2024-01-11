using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tenon.Redis;

public interface IRedisBfProvider
{
    /// <summary>
    ///     https://redis.io/commands/bf.add/
    /// </summary>
    Task<bool> BfAddAsync(string key, string value);

    /// <summary>
    ///     https://redis.io/commands/bf.madd/
    /// </summary>
    Task<bool[]> BfAddAsync(string key, IEnumerable<string> values);

    /// <summary>
    ///     https://redis.io/commands/bf.exists/
    /// </summary>
    Task<bool> BfExistsAsync(string key, string value);

    /// <summary>
    ///     https://redis.io/commands/bf.info/
    /// </summary>
    Task<long> BfInfoAsync(string key);

    /// <summary>
    ///     https://redis.io/commands/bf.mexists/
    /// </summary>
    Task<bool[]> BfExistsAsync(string key, IEnumerable<string> values);

    /// <summary>
    ///     https://redis.io/commands/bf.reserve/
    /// </summary>
    Task BfReserveAsync(string key, double errorRate, int initialCapacity);
}