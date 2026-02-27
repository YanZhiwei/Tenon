using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.Caching.Abstractions;
using Tenon.Caching.Abstractions.Configurations;

namespace Tenon.Caching.InMemory.Extensions;

/// <summary>
/// 内存缓存的 <see cref="ICachingOptionsExtension" /> 实现，用于在 <see cref="CachingOptions" /> 上通过 UseInMemoryStorage 注册内存缓存服务。
/// </summary>
public sealed class CachingOptionsExtension : ICachingOptionsExtension
{
    private readonly CachingOptions _options;

    /// <summary>
    /// 使用指定的 <see cref="CachingOptions" /> 创建扩展实例。
    /// </summary>
    /// <param name="options">缓存全局选项（如 KeyedServiceKey）。</param>
    public CachingOptionsExtension(CachingOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// 将内存缓存服务注册到 <paramref name="services" />：根据 <see cref="CachingOptions.KeyedServiceKey" /> 注册为单例或键控单例 <see cref="ICacheProvider" />。
    /// </summary>
    /// <param name="services">要注册服务的 <see cref="IServiceCollection" />。</param>
    public void AddServices(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (string.IsNullOrWhiteSpace(_options.KeyedServiceKey))
        {
            services.TryAddSingleton<ICacheProvider, MemoryCacheProvider>();
        }
        else
        {
            var key = _options.KeyedServiceKey;
            services.TryAddKeyedSingleton<ICacheProvider>(key, (_, _) => new MemoryCacheProvider());
        }
    }
}
