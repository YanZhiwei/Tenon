using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.Caching.Abstractions;

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
}
