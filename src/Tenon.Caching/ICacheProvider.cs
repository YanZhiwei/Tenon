using System;
using System.Threading.Tasks;

namespace Tenon.Caching
{
    public interface ICacheProvider
    {
        void Set<T>(string cacheKey, T cacheValue, TimeSpan expiration);

        Task SetAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration);
    }
}