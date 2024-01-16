using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.Caching.Redis;
using Tenon.Caching.Redis.Configurations;
using Tenon.Redis.Configurations;
using Tenon.Redis.StackExchangeProvider.Extensions;
using Tenon.Serialization.Json.Extensions;

namespace Tenon.Caching.RedisStackExchange.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddKeyedRedisStackExchangeCache(this IServiceCollection services,
        string serviceKey,
        IConfigurationSection redisCacheSection)
    {
        if (string.IsNullOrWhiteSpace(serviceKey))
            throw new ArgumentNullException(nameof(serviceKey));
        if (redisCacheSection == null)
            throw new ArgumentNullException(nameof(redisCacheSection));
        var redisCacheConfig = redisCacheSection.Get<RedisCachingOptions>();
        if (redisCacheConfig == null)
            throw new ArgumentNullException(nameof(redisCacheConfig));
        if (redisCacheConfig.Redis == null)
            throw new ArgumentNullException(nameof(redisCacheConfig.Redis));
        var redisSection = redisCacheSection.GetSection(nameof(redisCacheConfig.Redis));
        var redisConfig = redisSection.Get<RedisOptions>();
        if (redisConfig == null)
            throw new ArgumentNullException(nameof(redisConfig));
        if (string.IsNullOrWhiteSpace(redisConfig.ConnectionString))
            throw new ArgumentNullException(nameof(redisConfig.ConnectionString));
        services.AddKeyedSystemTextJsonSerializer(serviceKey);
        services.AddKeyedRedisStackExchangeProvider(serviceKey, redisCacheSection);
        services.TryAddKeyedScoped<ICacheProvider, RedisCacheProvider>(serviceKey);
        return services;
    }

    public static IServiceCollection AddRedisStackExchangeCache(this IServiceCollection services,
        IConfigurationSection redisCacheSection)
    {
        if (redisCacheSection == null)
            throw new ArgumentNullException(nameof(redisCacheSection));
        var redisCacheConfig = redisCacheSection.Get<RedisCachingOptions>();
        if (redisCacheConfig == null)
            throw new ArgumentNullException(nameof(redisCacheConfig));
        if (redisCacheConfig.Redis == null)
            throw new ArgumentNullException(nameof(redisCacheConfig.Redis));
        var redisSection = redisCacheSection.GetSection(nameof(redisCacheConfig.Redis));
        var redisConfig = redisSection.Get<RedisOptions>();
        if (redisConfig == null)
            throw new ArgumentNullException(nameof(redisConfig));
        if (string.IsNullOrWhiteSpace(redisConfig.ConnectionString))
            throw new ArgumentNullException(nameof(redisConfig.ConnectionString));
        services.Configure<RedisCachingOptions>(redisCacheSection);
        services.Configure<RedisOptions>(redisSection);
        services.AddSystemTextJsonSerializer();
        services.AddRedisStackExchangeProvider(redisCacheSection);
        services.TryAddSingleton<ICacheProvider, RedisCacheProvider>();
        return services;
    }
}