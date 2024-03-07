using Tenon.Caching.Abstractions;

namespace Tenon.Caching.InMemory
{
    public sealed class MemoryCacheProvider: ICacheProvider
    {
        public bool Set<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public CacheValue<T> Get<T>(string cacheKey)
        {
            throw new NotImplementedException();
        }

        public Task<CacheValue<T>> GetAsync<T>(string cacheKey)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string cacheKey)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(string cacheKey)
        {
            throw new NotImplementedException();
        }


        public Task<bool> ExistsAsync(string cacheKey)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string cacheKey)
        {
            throw new NotImplementedException();
        }

        public long RemoveAll(IEnumerable<string> cacheKeys)
        {
            throw new NotImplementedException();
        }

        public Task<long> RemoveAllAsync(IEnumerable<string> cacheKeys)
        {
            throw new NotImplementedException();
        }


        public Task KeysExpireAsync(IEnumerable<string> cacheKeys)
        {
            throw new NotImplementedException();
        }
    }
}
