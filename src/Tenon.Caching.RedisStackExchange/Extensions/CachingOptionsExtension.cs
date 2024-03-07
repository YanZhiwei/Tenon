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

namespace Tenon.Caching.RedisStackExchange.Extensions;

internal class CachingOptionsExtension(IConfigurationSection redisSection, CachingOptions options)
    : ICachingOptionsExtension
{
    private readonly CachingOptions _options = options ?? throw new ArgumentNullException(nameof(options));
    private readonly IConfigurationSection _redisSection = redisSection ?? throw new ArgumentNullException(nameof(redisSection));

    public void AddServices(IServiceCollection services)
    {
        var redisConfig = _redisSection.Get<RedisOptions>();
        if (redisConfig == null)
            throw new ArgumentNullException(nameof(redisConfig));
        if (string.IsNullOrWhiteSpace(redisConfig.ConnectionString))
            throw new ArgumentNullException(nameof(redisConfig.ConnectionString));
        if (!_options.KeyedServices)
        {
            services.AddRedisStackExchangeProvider(_redisSection);
            services.TryAddSingleton<ICacheProvider, RedisCacheProvider>();
        }
        else
        {
            var serviceKey = _options.KeyedServiceKey;
            if (string.IsNullOrWhiteSpace(serviceKey))
                throw new ArgumentNullException(nameof(_options.KeyedServiceKey));
            services.AddKeyedRedisStackExchangeProvider(serviceKey, _redisSection);
            services.TryAddKeyedSingleton<ICacheProvider>(serviceKey, (serviceProvider, key) =>
            {
                var redisProvider = serviceProvider.GetKeyedService<IRedisProvider>(key);
                var serializer = serviceProvider.GetKeyedService<ISerializer>(key);
                return new RedisCacheProvider(redisProvider, serializer, _options);
            });
        }
    }
}