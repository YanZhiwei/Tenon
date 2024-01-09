using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tenon.Redis
{
    public interface IRedisSetProvider
    {
        /// <summary>
        /// https://redis.io/commands/sadd
        /// </summary>
        long SAdd<T>(string cacheKey, IEnumerable<T> cacheValues, TimeSpan? expiration = null);

        /// <summary>
        /// https://redis.io/commands/scard
        /// </summary>
        long SCard(string cacheKey);

        /// <summary>
        /// https://redis.io/commands/sismember
        /// </summary>
        bool SIsMember<T>(string cacheKey, T cacheValue);

        /// <summary>
        /// https://redis.io/commands/smembers
        /// </summary>
        List<T> SMembers<T>(string cacheKey);

        /// <summary>
        /// https://redis.io/commands/spop
        /// </summary>
        T SPop<T>(string cacheKey);

        /// <summary>
        /// https://redis.io/commands/srandmember
        /// </summary>
        IEnumerable<T> SRandMember<T>(string cacheKey, int count = 1);

        /// <summary>
        /// https://redis.io/commands/srem
        /// </summary>
        long SRem<T>(string cacheKey, IEnumerable<T> cacheValues = null);

        /// <summary>
        /// https://redis.io/commands/sadd
        /// </summary>
        Task<long> SAddAsync<T>(string cacheKey, IEnumerable<T> cacheValues, TimeSpan? expiration = null);

        /// <summary>
        /// https://redis.io/commands/scard
        /// </summary>
        Task<long> SCardAsync(string cacheKey);

        /// <summary>
        /// https://redis.io/commands/sismember
        /// </summary>
        Task<bool> SIsMemberAsync<T>(string cacheKey, T cacheValue);

        /// <summary>
        /// https://redis.io/commands/smembers
        /// </summary>
        Task<IEnumerable<T>> SMembersAsync<T>(string cacheKey);

        /// <summary>
        /// https://redis.io/commands/spop
        /// </summary>
        Task<T> SPopAsync<T>(string cacheKey);

        /// <summary>
        /// https://redis.io/commands/srandmember
        /// </summary>
        Task<List<T>> SRandMemberAsync<T>(string cacheKey, int count = 1);

        /// <summary>
        /// https://redis.io/commands/srem
        /// </summary>
        Task<long> SRemAsync<T>(string cacheKey, IEnumerable<T> cacheValues = null);
    }
}
