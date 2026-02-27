using System.Collections.Specialized;
using System.Runtime.Caching;
using Tenon.Caching.Abstractions;

namespace Tenon.Caching.InMemory;

/// <summary>
/// 基于 <see cref="System.Runtime.Caching.MemoryCache" /> 的内存缓存提供程序，实现 <see cref="ICacheProvider" />。
/// 支持使用默认缓存或自定义名称与策略（内存限制、轮询间隔等）。
/// </summary>
public sealed class MemoryCacheProvider : ICacheProvider, IDisposable
{
    private const int DefaultPollingIntervalMinutes = 2;

    private readonly MemoryCache _cache;
    private readonly bool _isDefaultCache;

    /// <summary>
    /// 创建内存缓存实例。不传任何参数时使用 <see cref="MemoryCache.Default" />；传入任一参数则创建具名缓存并应用对应策略。
    /// </summary>
    /// <param name="cacheName">缓存实例名称；为 null 且其余参数均为 null 时使用默认缓存。</param>
    /// <param name="cacheMemoryLimitMegabytes">缓存最大内存限制（MB）。</param>
    /// <param name="physicalMemoryLimitPercentage">占物理内存的百分比上限（0–100）。</param>
    /// <param name="pollingInterval">过期项轮询清理间隔；未指定时默认 2 分钟。</param>
    public MemoryCacheProvider(string? cacheName = null, long? cacheMemoryLimitMegabytes = null,
        int? physicalMemoryLimitPercentage = null, TimeSpan? pollingInterval = null)
    {
        if (cacheName == null && cacheMemoryLimitMegabytes == null && physicalMemoryLimitPercentage == null &&
            pollingInterval == null)
        {
            _cache = MemoryCache.Default;
            _isDefaultCache = true;
        }
        else
        {
            var config = new NameValueCollection();
            if (cacheMemoryLimitMegabytes.HasValue)
                config.Add("CacheMemoryLimitMegabytes", cacheMemoryLimitMegabytes.Value.ToString());
            if (physicalMemoryLimitPercentage.HasValue)
                config.Add("PhysicalMemoryLimitPercentage", physicalMemoryLimitPercentage.Value.ToString());
            config.Add("PollingInterval", (pollingInterval ?? TimeSpan.FromMinutes(DefaultPollingIntervalMinutes)).ToString());

            _cache = new MemoryCache(cacheName ?? nameof(MemoryCacheProvider), config);
            _isDefaultCache = false;
        }
    }

    /// <summary>
    /// 将指定键和值写入缓存，并设置绝对过期时间。
    /// </summary>
    /// <param name="cacheKey">缓存键。</param>
    /// <param name="cacheValue">要缓存的值。</param>
    /// <param name="expiration">过期时间间隔。</param>
    /// <returns>写入成功返回 true；键或值为空时返回 false。</returns>
    public bool Set<T>(string cacheKey, T cacheValue, TimeSpan expiration)
    {
        if (string.IsNullOrEmpty(cacheKey) || cacheValue == null)
            return false;

        var policy = new CacheItemPolicy
        {
            AbsoluteExpiration = DateTimeOffset.UtcNow.Add(expiration)
        };
        _cache.Set(cacheKey, cacheValue, policy);
        return true;
    }

    /// <summary>
    /// 异步将指定键和值写入缓存，并设置绝对过期时间。
    /// </summary>
    /// <param name="cacheKey">缓存键。</param>
    /// <param name="cacheValue">要缓存的值。</param>
    /// <param name="expiration">过期时间间隔。</param>
    /// <returns>写入成功返回 true；键或值为空时返回 false。</returns>
    public Task<bool> SetAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration)
    {
        return Task.FromResult(Set(cacheKey, cacheValue, expiration));
    }

    /// <summary>
    /// 根据键获取缓存值。
    /// </summary>
    /// <param name="cacheKey">缓存键。</param>
    /// <returns>命中返回包含值的 <see cref="CacheValue{T}" />，未命中返回 <see cref="CacheValue{T}.Null" />。</returns>
    public CacheValue<T> Get<T>(string cacheKey)
    {
        var value = _cache.Get(cacheKey);
        return value is T typedValue
            ? new CacheValue<T>(typedValue, true)
            : CacheValue<T>.Null;
    }

    /// <summary>
    /// 异步根据键获取缓存值。
    /// </summary>
    /// <param name="cacheKey">缓存键。</param>
    /// <returns>命中返回包含值的 <see cref="CacheValue{T}" />，未命中返回 <see cref="CacheValue{T}.Null" />。</returns>
    public Task<CacheValue<T>> GetAsync<T>(string cacheKey)
    {
        return Task.FromResult(Get<T>(cacheKey));
    }

    /// <summary>
    /// 移除指定键的缓存项。
    /// </summary>
    /// <param name="cacheKey">缓存键。</param>
    /// <returns>存在并移除返回 true，否则返回 false。</returns>
    public bool Remove(string cacheKey)
    {
        return _cache.Remove(cacheKey) != null;
    }

    /// <summary>
    /// 异步移除指定键的缓存项。
    /// </summary>
    /// <param name="cacheKey">缓存键。</param>
    /// <returns>存在并移除返回 true，否则返回 false。</returns>
    public Task<bool> RemoveAsync(string cacheKey)
    {
        return Task.FromResult(Remove(cacheKey));
    }

    /// <summary>
    /// 判断指定键的缓存项是否存在。
    /// </summary>
    /// <param name="cacheKey">缓存键。</param>
    /// <returns>存在返回 true，否则返回 false。</returns>
    public bool Exists(string cacheKey)
    {
        return _cache.Contains(cacheKey);
    }

    /// <summary>
    /// 异步判断指定键的缓存项是否存在。
    /// </summary>
    /// <param name="cacheKey">缓存键。</param>
    /// <returns>存在返回 true，否则返回 false。</returns>
    public Task<bool> ExistsAsync(string cacheKey)
    {
        return Task.FromResult(Exists(cacheKey));
    }

    /// <summary>
    /// 批量移除指定键的缓存项。
    /// </summary>
    /// <param name="cacheKeys">缓存键集合。</param>
    /// <returns>实际移除的项数。</returns>
    public long RemoveAll(IEnumerable<string> cacheKeys)
    {
        var keys = cacheKeys as ICollection<string> ?? cacheKeys.ToList();
        return keys.Count(key => Remove(key));
    }

    /// <summary>
    /// 异步批量移除指定键的缓存项。
    /// </summary>
    /// <param name="cacheKeys">缓存键集合。</param>
    /// <returns>实际移除的项数。</returns>
    public Task<long> RemoveAllAsync(IEnumerable<string> cacheKeys)
    {
        return Task.FromResult(RemoveAll(cacheKeys));
    }

    /// <summary>
    /// 使指定键的缓存项立即过期（移除）。
    /// </summary>
    /// <param name="cacheKeys">要过期的缓存键集合。</param>
    /// <returns>表示异步完成的任务。</returns>
    public Task KeysExpireAsync(IEnumerable<string> cacheKeys)
    {
        return Task.WhenAll(cacheKeys.Select(key => Task.Run(() => _cache.Remove(key))));
    }

    /// <summary>
    /// 将指定键的缓存项过期时间更新为新的间隔。
    /// </summary>
    /// <param name="cacheKeys">要更新过期时间的缓存键集合。</param>
    /// <param name="expiration">新的过期时间间隔。</param>
    /// <returns>表示异步完成的任务。</returns>
    public Task KeysExpireAsync(IEnumerable<string> cacheKeys, TimeSpan expiration)
    {
        var keys = cacheKeys as ICollection<string> ?? cacheKeys.ToList();
        foreach (var key in keys)
        {
            if (_cache.Get(key) is { } value)
                Set(key, value, expiration);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 释放由本实例占用的非默认缓存资源；使用 <see cref="MemoryCache.Default" /> 时不会执行释放。
    /// </summary>
    public void Dispose()
    {
        if (!_isDefaultCache)
            _cache.Dispose();
    }
}