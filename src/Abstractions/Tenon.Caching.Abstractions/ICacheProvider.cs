using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tenon.Caching.Abstractions;

public interface ICacheProvider
{
    bool Set<T>(string cacheKey, T cacheValue, TimeSpan expiration);
    Task<bool> SetAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration);
    CacheValue<T> Get<T>(string cacheKey);
    Task<CacheValue<T>> GetAsync<T>(string cacheKey);
    bool Remove(string cacheKey);
    Task<bool> RemoveAsync(string cacheKey);
    Task<bool> ExistsAsync(string cacheKey);
    bool Exists(string cacheKey);
    long RemoveAll(IEnumerable<string> cacheKeys);
    Task<long> RemoveAllAsync(IEnumerable<string> cacheKeys);
    Task KeysExpireAsync(IEnumerable<string> cacheKeys);

    Task KeysExpireAsync(IEnumerable<string> cacheKeys, TimeSpan expiration);
}