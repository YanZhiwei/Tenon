using Microsoft.Extensions.Configuration;
using Tenon.Caching.Abstractions.Configurations;
using Tenon.Caching.InMemory.Extensions;

namespace Tenon.Caching.InMemory.Configurations;

/// <summary>
/// 内存缓存相关的 <see cref="CachingOptions" /> 扩展方法。
/// </summary>
public static class InMemoryCachingOptions
{
    /// <summary>
    /// 将缓存存储配置为内存实现（使用 <see cref="MemoryCacheProvider" />）。
    /// </summary>
    /// <param name="options">缓存全局选项。</param>
    /// <param name="inMemorySection">可选：用于绑定内存缓存配置的配置节；当前未使用，保留用于后续扩展。</param>
    /// <returns>当前 <see cref="CachingOptions" />，便于链式调用。</returns>
    public static CachingOptions UseInMemoryStorage(this CachingOptions options,
        IConfigurationSection? inMemorySection = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.RegisterExtension(new CachingOptionsExtension(options));
        return options;
    }
}