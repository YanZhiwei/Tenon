using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.Caching.Abstractions;
using Tenon.Caching.Abstractions.Configurations;
using Tenon.Caching.Redis;
using Tenon.Infra.Redis;
using Tenon.Infra.Redis.Configurations;
using Tenon.Infra.Redis.StackExchangeProvider.Extensions;
using Tenon.Serialization.Abstractions;
using Tenon.Serialization.Json.Extensions;

namespace Tenon.Caching.RedisStackExchange.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddKeyedRedisStackExchangeCache(this IServiceCollection services,
        string serviceKey,
        IConfigurationSection redisSection, Action<CachingOptions>? setupAction = null)
    {
        if (string.IsNullOrWhiteSpace(serviceKey))
            throw new ArgumentNullException(nameof(serviceKey));
        if (redisSection == null)
            throw new ArgumentNullException(nameof(redisSection));
        var redisConfig = redisSection.Get<RedisOptions>();
        if (redisConfig == null)
            throw new ArgumentNullException(nameof(redisConfig));
        if (string.IsNullOrWhiteSpace(redisConfig.ConnectionString))
            throw new ArgumentNullException(nameof(redisConfig.ConnectionString));
        var options = new CachingOptions();
        if (setupAction != null)
            setupAction(options);
        services.AddKeyedSystemTextJsonSerializer(serviceKey);
        services.AddKeyedRedisStackExchangeProvider(serviceKey, redisSection);
        services.TryAddKeyedSingleton<ICacheProvider>(serviceKey, (serviceProvider, key) =>
        {
            var redisProvider = serviceProvider.GetKeyedService<IRedisProvider>(key);
            var serializer = serviceProvider.GetKeyedService<ISerializer>(key);
            return new RedisCacheProvider(redisProvider, serializer, options);
        });
        return services;
    }


    public static IServiceCollection AddRedisStackExchangeCache(this IServiceCollection services,
        IConfigurationSection redisSection, Action<CachingOptions>? setupAction = null)
    {
        if (redisSection == null)
            throw new ArgumentNullException(nameof(redisSection));
        var redisConfig = redisSection.Get<RedisOptions>();
        if (redisConfig == null)
            throw new ArgumentNullException(nameof(redisConfig));
        if (string.IsNullOrWhiteSpace(redisConfig.ConnectionString))
            throw new ArgumentNullException(nameof(redisConfig.ConnectionString));
        var options = new CachingOptions();
        if (setupAction != null)
            setupAction(options);
        services.Configure<RedisOptions>(redisSection);
        services.AddSystemTextJsonSerializer();
        services.AddSingleton(options);
        services.AddRedisStackExchangeProvider(redisSection);
        services.TryAddSingleton<ICacheProvider, RedisCacheProvider>();
        return services;
    }
}