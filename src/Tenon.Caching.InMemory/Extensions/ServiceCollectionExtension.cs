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
    /// 注册默认内存缓存：使用 <see cref="MemoryCache.Default" /> 的 <see cref="MemoryCacheProvider" /> 单例。
    /// 适用于简单场景；需键控服务或通过 CachingOptions 统一配置时，请使用 <see cref="Configurations.InMemoryCachingOptions.UseInMemoryStorage" />。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <returns>当前 <see cref="IServiceCollection" />，便于链式调用。</returns>
    public static IServiceCollection AddInMemoryCache(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.TryAddSingleton<ICacheProvider, MemoryCacheProvider>();
        return services;
    }

    /// <summary>
    /// 注册内存缓存，并可选的通过 <paramref name="configureOptions" /> 配置缓存名称、内存限制与轮询间隔。
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
            services.TryAddSingleton<ICacheProvider, MemoryCacheProvider>();
            return services;
        }

        var options = new InMemoryCacheOptions();
        configureOptions(options);
        var provider = new MemoryCacheProvider(
            options.CacheName,
            options.CacheMemoryLimitMegabytes,
            options.PhysicalMemoryLimitPercentage,
            options.PollingInterval);
        services.TryAddSingleton<ICacheProvider>(provider);
        return services;
    }
}
