namespace Tenon.Caching.InMemory.Configurations;

/// <summary>
/// 内存缓存的配置选项，与 <see cref="MemoryCacheProvider" /> 构造函数参数对应。
/// 用于 <see cref="Extensions.ServiceCollectionExtension.AddInMemoryCache(Microsoft.Extensions.DependencyInjection.IServiceCollection,System.Action{InMemoryCacheOptions}?)" /> 重载。
/// </summary>
public sealed class InMemoryCacheOptions
{
    /// <summary>
    /// 缓存实例名称；为 null 且其余选项均为 null 时使用 <see cref="MemoryCache.Default" />。
    /// </summary>
    public string? CacheName { get; set; }

    /// <summary>
    /// 缓存最大内存限制（MB）。
    /// </summary>
    public long? CacheMemoryLimitMegabytes { get; set; }

    /// <summary>
    /// 占物理内存的百分比上限（0–100）。
    /// </summary>
    public int? PhysicalMemoryLimitPercentage { get; set; }

    /// <summary>
    /// 过期项轮询清理间隔；未设置时默认 2 分钟。
    /// </summary>
    public TimeSpan? PollingInterval { get; set; }
}
