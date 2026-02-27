namespace Tenon.Caching.InMemory.Configurations;

/// <summary>
/// 内存缓存的配置选项，用于 <see cref="Extensions.ServiceCollectionExtension.AddInMemoryCache(Microsoft.Extensions.DependencyInjection.IServiceCollection,System.Action{InMemoryCacheOptions}?)" /> 重载。
/// 映射到 <see cref="Microsoft.Extensions.Caching.Memory.MemoryCacheOptions" />（SizeLimit、ExpirationScanFrequency 等）。
/// </summary>
public sealed class InMemoryCacheOptions
{
    /// <summary>
    /// 缓存实例名称；用于创建独立缓存实例时的标识（Microsoft.Extensions.Caching.Memory 无全局“名称”概念，仅区分实例）。
    /// </summary>
    public string? CacheName { get; set; }

    /// <summary>
    /// 缓存大小上限（与 <see cref="MemoryCacheOptions.SizeLimit" /> 对应）；设值时写入的项将按单位 1 计入，即近似“最大条数”。
    /// </summary>
    public long? CacheMemoryLimitMegabytes { get; set; }

    /// <summary>
    /// 占物理内存的百分比上限（0–100）。Microsoft.Extensions.Caching.Memory 不支持此选项，保留属性以兼容配置，当前不生效。
    /// </summary>
    public int? PhysicalMemoryLimitPercentage { get; set; }

    /// <summary>
    /// 过期项轮询清理间隔，对应 <see cref="MemoryCacheOptions.ExpirationScanFrequency" />；未设置时默认 2 分钟。
    /// </summary>
    public TimeSpan? PollingInterval { get; set; }
}
