using Microsoft.Extensions.Caching.Memory;
using Tenon.Caching.Abstractions;

namespace Tenon.Caching.InMemory;

/// <summary>
/// 基于 <see cref="IMemoryCache" />（Microsoft.Extensions.Caching.Memory）的内存缓存提供程序，实现 <see cref="ICacheProvider" />。
/// </summary>
public sealed class MemoryCacheProvider : ICacheProvider, IDisposable
{
    private readonly IMemoryCache _cache;
    private readonly IDisposable? _ownedCache;
    private readonly bool _useSizeLimit;

    /// <summary>
    /// 使用外部注入的 <see cref="IMemoryCache" /> 创建提供程序；不持有所有权，Dispose 时不释放该缓存。
    /// </summary>
    /// <param name="cache">内存缓存实例（通常来自 DI 的 <see cref="IMemoryCache" />）。</param>
    public MemoryCacheProvider(IMemoryCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _ownedCache = null;
        _useSizeLimit = false;
    }

    /// <summary>
    /// 使用指定的 <see cref="MemoryCacheOptions" /> 创建独立缓存实例；本实例持有所有权，Dispose 时释放该缓存。
    /// </summary>
    /// <param name="options">缓存选项。</param>
    public MemoryCacheProvider(MemoryCacheOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        var inner = new MemoryCache(options);
        _cache = inner;
        _ownedCache = inner;
        _useSizeLimit = options.SizeLimit.HasValue;
    }

    /// <summary>
    /// 将指定键和值写入缓存，并设置绝对过期时间。
    /// </summary>
    public bool Set<T>(string cacheKey, T cacheValue, TimeSpan expiration)
    {
        if (string.IsNullOrEmpty(cacheKey) || cacheValue == null)
            return false;

        var entryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };
        if (_useSizeLimit)
            entryOptions.Size = 1;

        _cache.Set(cacheKey, cacheValue, entryOptions);
        return true;
    }

    /// <summary>
    /// 异步将指定键和值写入缓存，并设置绝对过期时间。
    /// </summary>
    /// <param name="cacheKey">缓存键。</param>
    /// <param name="cacheValue">要缓存的值。</param>
    /// <param name="expiration">绝对过期时间。</param>
    /// <returns>写入成功为 true，否则为 false。</returns>
    public Task<bool> SetAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration)
    {
        return Task.FromResult(Set(cacheKey, cacheValue, expiration));
    }

    /// <summary>
    /// 根据缓存键获取值；不存在或类型不匹配时返回 <see cref="CacheValue{T}.Null" />。
    /// </summary>
    /// <param name="cacheKey">缓存键。</param>
    /// <returns>包含值及是否命中的 <see cref="CacheValue{T}" />。</returns>
    public CacheValue<T> Get<T>(string cacheKey)
    {
        if (string.IsNullOrEmpty(cacheKey))
            return CacheValue<T>.Null;

        return _cache.TryGetValue(cacheKey, out object? value) && value is T typedValue
            ? new CacheValue<T>(typedValue, true)
            : CacheValue<T>.Null;
    }

    /// <summary>
    /// 异步根据缓存键获取值；不存在或类型不匹配时返回 <see cref="CacheValue{T}.Null" />。
    /// </summary>
    /// <param name="cacheKey">缓存键。</param>
    /// <returns>包含值及是否命中的 <see cref="CacheValue{T}" />。</returns>
    public Task<CacheValue<T>> GetAsync<T>(string cacheKey)
    {
        return Task.FromResult(Get<T>(cacheKey));
    }

    /// <summary>
    /// 移除指定键的缓存项。
    /// </summary>
    /// <param name="cacheKey">缓存键。</param>
    /// <returns>若该键原本存在则为 true，否则为 false。</returns>
    public bool Remove(string cacheKey)
    {
        if (string.IsNullOrEmpty(cacheKey))
            return false;

        var existed = _cache.TryGetValue(cacheKey, out _);
        _cache.Remove(cacheKey);
        return existed;
    }

    /// <summary>
    /// 异步移除指定键的缓存项。
    /// </summary>
    /// <param name="cacheKey">缓存键。</param>
    /// <returns>若该键原本存在则为 true，否则为 false。</returns>
    public Task<bool> RemoveAsync(string cacheKey)
    {
        return Task.FromResult(Remove(cacheKey));
    }

    /// <summary>
    /// 检查指定键的缓存项是否存在。
    /// </summary>
    /// <param name="cacheKey">缓存键。</param>
    /// <returns>存在为 true，否则为 false。</returns>
    public bool Exists(string cacheKey)
    {
        return !string.IsNullOrEmpty(cacheKey) && _cache.TryGetValue(cacheKey, out _);
    }

    /// <summary>
    /// 异步检查指定键的缓存项是否存在。
    /// </summary>
    /// <param name="cacheKey">缓存键。</param>
    /// <returns>存在为 true，否则为 false。</returns>
    public Task<bool> ExistsAsync(string cacheKey)
    {
        return Task.FromResult(Exists(cacheKey));
    }

    /// <summary>
    /// 移除多个缓存键对应的项。
    /// </summary>
    /// <param name="cacheKeys">要移除的缓存键集合。</param>
    /// <returns>实际被移除的项数量。</returns>
    public long RemoveAll(IEnumerable<string> cacheKeys)
    {
        var keys = cacheKeys as ICollection<string> ?? cacheKeys.ToList();
        return keys.Count(key => Remove(key));
    }

    /// <summary>
    /// 异步移除多个缓存键对应的项。
    /// </summary>
    /// <param name="cacheKeys">要移除的缓存键集合。</param>
    /// <returns>实际被移除的项数量。</returns>
    public Task<long> RemoveAllAsync(IEnumerable<string> cacheKeys)
    {
        return Task.FromResult(RemoveAll(cacheKeys));
    }

    /// <summary>
    /// 异步使指定键的缓存项立即过期（移除）。
    /// </summary>
    /// <param name="cacheKeys">要过期的缓存键集合。</param>
    /// <returns>表示异步完成的任务。</returns>
    public Task KeysExpireAsync(IEnumerable<string> cacheKeys)
    {
        var keys = cacheKeys as ICollection<string> ?? cacheKeys.ToList();
        foreach (var key in keys)
            _cache.Remove(key);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 异步将指定键的缓存项设置为在给定时间后过期（先取出现值再按新过期时间写回）。
    /// </summary>
    /// <param name="cacheKeys">要更新过期时间的缓存键集合。</param>
    /// <param name="expiration">新的绝对过期时间。</param>
    /// <returns>表示异步完成的任务。</returns>
    public Task KeysExpireAsync(IEnumerable<string> cacheKeys, TimeSpan expiration)
    {
        var keys = cacheKeys as ICollection<string> ?? cacheKeys.ToList();
        foreach (var key in keys)
        {
            if (_cache.TryGetValue(key, out object? value))
                Set(key, value, expiration);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 释放本实例持有的缓存资源（仅当由 <see cref="MemoryCacheProvider(MemoryCacheOptions)" /> 创建时才会释放内部缓存）。
    /// </summary>
    public void Dispose()
    {
        _ownedCache?.Dispose();
    }
}
