using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.Caching.Redis.Configurations;
using Tenon.Serialization;

namespace Tenon.Caching.Redis.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddRedisCache<TSerializer>(this IServiceCollection services,
        IConfigurationSection redisCacheSection)
        where TSerializer : class, ISerializer
    {
        if (redisCacheSection == null)
            throw new ArgumentNullException(nameof(redisCacheSection));
        var redisCacheConfig = redisCacheSection.Get<RedisCachingOptions>();
        if (redisCacheConfig == null)
            throw new ArgumentNullException(nameof(redisCacheConfig));
        services.Configure<RedisCachingOptions>(redisCacheSection);
        services.TryAddSingleton<ISerializer, TSerializer>();
        services.TryAddSingleton<ICacheProvider, RedisCacheProvider>();
        return services;
    }
}