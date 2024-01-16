using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.Caching.Redis.Configurations;
using Tenon.Redis;
using Tenon.Serialization;

namespace Tenon.Caching.Redis.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddKeyedRedisCache(this IServiceCollection services, string serviceKey,
        IConfigurationSection redisCacheSection, bool requiredKeyedService = false)
    {
        if (string.IsNullOrEmpty(serviceKey))
            throw new ArgumentNullException(nameof(serviceKey));
        if (redisCacheSection == null)
            throw new ArgumentNullException(nameof(redisCacheSection));
        var redisCacheConfig = redisCacheSection.Get<RedisCachingOptions>();
        if (redisCacheConfig == null)
            throw new ArgumentNullException(nameof(redisCacheConfig));
        services.TryAddKeyedSingleton<ICacheProvider>(serviceKey, (serviceProvider, key) =>
        {
            var redisProvider = serviceProvider.GetKeyedService<IRedisProvider>(key);
            if (redisProvider == null && !requiredKeyedService)
                redisProvider = serviceProvider.GetService<IRedisProvider>();
            var serializer = serviceProvider.GetKeyedService<ISerializer>(key);
            if (serializer == null && !requiredKeyedService)
                serializer = serviceProvider.GetService<ISerializer>();
#pragma warning disable CS8604 // Possible null reference argument.
            return new RedisCacheProvider(redisCacheConfig, redisProvider, serializer);
#pragma warning restore CS8604 // Possible null reference argument.
        });
        return services;
    }

    public static IServiceCollection AddRedisCache(this IServiceCollection services,
        IConfigurationSection redisCacheSection)
    {
        if (redisCacheSection == null)
            throw new ArgumentNullException(nameof(redisCacheSection));
        var redisCacheConfig = redisCacheSection.Get<RedisCachingOptions>();
        if (redisCacheConfig == null)
            throw new ArgumentNullException(nameof(redisCacheConfig));
        services.Configure<RedisCachingOptions>(redisCacheSection);
        services.TryAddSingleton<ICacheProvider, RedisCacheProvider>();
        return services;
    }
}