using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.Caching.Configurations;
using Tenon.Caching.Redis;
using Tenon.Caching.Redis.Configurations;
using Tenon.Infra.Redis;
using Tenon.Infra.Redis.Configurations;
using Tenon.Infra.Redis.StackExchangeProvider.Extensions;
using Tenon.Serialization.Abstractions;

namespace Tenon.Caching.RedisStackExchange.Extensions;

internal class CachingOptionsExtension(IConfigurationSection redisCacheSection, CachingOptions options)
    : ICachingOptionsExtension
{
    public void AddServices(IServiceCollection services)
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
        if (!options.KeyedServices)
        {
            services.AddRedisStackExchangeProvider(redisSection);
            services.TryAddSingleton<ICacheProvider, RedisCacheProvider>();
        }
        else
        {
            var serviceKey = options.KeyedServiceKey;
            if (string.IsNullOrWhiteSpace(serviceKey))
                throw new ArgumentNullException(nameof(options.KeyedServiceKey));
            services.AddKeyedRedisStackExchangeProvider(serviceKey, redisSection);
            services.TryAddKeyedSingleton<ICacheProvider>(serviceKey, (serviceProvider, key) =>
            {
                var redisProvider = serviceProvider.GetKeyedService<IRedisProvider>(key);
                var serializer = serviceProvider.GetRequiredKeyedService<ISerializer>(key);
                return new RedisCacheProvider(redisCacheConfig, redisProvider, serializer);
            });
        }
    }
}