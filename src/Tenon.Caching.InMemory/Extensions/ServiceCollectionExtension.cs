using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.Caching.Abstractions;
using Tenon.Caching.InMemory.Configurations;

namespace Tenon.Caching.InMemory.Extensions;

/// <summary>
/// <see cref="IServiceCollection" /> 的内存缓存服务注册扩展。
/// </summary>
public static class ServiceCollectionExtension
{
    /// <summary>
    /// 注册默认内存缓存：使用 <see cref="IMemoryCache" />（通过 <see cref="MemoryCacheServiceCollectionExtensions.AddMemoryCache" />）的 <see cref="MemoryCacheProvider" /> 单例。
    /// 适用于简单场景；需键控服务或通过 CachingOptions 统一配置时，请使用 <see cref="CachingOptionsInMemoryExtensions.UseInMemoryStorage" />。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <returns>当前 <see cref="IServiceCollection" />，便于链式调用。</returns>
    public static IServiceCollection AddInMemoryCache(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddMemoryCache();
        services.TryAddSingleton<ICacheProvider>(sp => new MemoryCacheProvider(sp.GetRequiredService<IMemoryCache>()));
        return services;
    }

    /// <summary>
    /// 注册内存缓存，并可选的通过 <paramref name="configureOptions" /> 配置大小限制与过期扫描间隔。
    /// 当 <paramref name="configureOptions" /> 为 null 时行为与无参 <see cref="AddInMemoryCache(IServiceCollection)" /> 一致。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configureOptions">可选；用于配置 <see cref="InMemoryCacheOptions" /> 的回调。</param>
    /// <returns>当前 <see cref="IServiceCollection" />，便于链式调用。</returns>
    public static IServiceCollection AddInMemoryCache(this IServiceCollection services,
        Action<InMemoryCacheOptions>? configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        if (configureOptions == null)
        {
            services.AddMemoryCache();
            services.TryAddSingleton<ICacheProvider>(sp => new MemoryCacheProvider(sp.GetRequiredService<IMemoryCache>()));
            return services;
        }

        var options = new InMemoryCacheOptions();
        configureOptions(options);
        var memoryOptions = ToMemoryCacheOptions(options);
        var provider = new MemoryCacheProvider(memoryOptions);
        services.TryAddSingleton<ICacheProvider>(provider);
        return services;
    }

    private static MemoryCacheOptions ToMemoryCacheOptions(InMemoryCacheOptions options)
    {
        var memoryOptions = new MemoryCacheOptions();
        if (options.PollingInterval.HasValue)
            memoryOptions.ExpirationScanFrequency = options.PollingInterval.Value;
        if (options.CacheMemoryLimitMegabytes.HasValue)
            memoryOptions.SizeLimit = options.CacheMemoryLimitMegabytes.Value;
        return memoryOptions;
    }
}
