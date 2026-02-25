using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.Caching.Abstractions;

namespace Tenon.Caching.InMemory.Extensions;

/// <summary>
///     内存缓存服务注册扩展
/// </summary>
public static class ServiceCollectionExtension
{
    /// <summary>
    ///     添加默认内存缓存（使用 MemoryCache.Default）
    /// </summary>
    public static IServiceCollection AddInMemoryCache(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.TryAddSingleton<ICacheProvider, MemoryCacheProvider>();
        return services;
    }
}
