using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.BloomFilter.Abstractions;
using Tenon.BloomFilter.Abstractions.Configurations;
using Tenon.BloomFilter.Redis;
using Tenon.Infra.Redis;
using Tenon.Infra.Redis.Configurations;
using Tenon.Infra.Redis.StackExchangeProvider.Extensions;

namespace Tenon.BloomFilter.RedisStackExchange.Extensions;

internal class BloomFilterOptionsExtension(IConfigurationSection redisSection, BloomFilterOptions options)
    : IBloomFilterOptionsExtension
{
    public void AddServices(IServiceCollection services)
    {
        if (redisSection == null)
            throw new ArgumentNullException(nameof(redisSection));
        var redisConfig = redisSection.Get<RedisOptions>();
        if (redisConfig == null)
            throw new ArgumentNullException(nameof(redisSection));
        if (string.IsNullOrWhiteSpace(options.KeyedServiceKey))
        {
            services.AddRedisStackExchangeProvider(redisSection);
            services.TryAddSingleton<IBloomFilter, RedisBloomFilter>();
        }
        else
        {
            var serviceKey = options.KeyedServiceKey;
            services.AddKeyedRedisStackExchangeProvider(serviceKey, redisSection);
            services.TryAddKeyedSingleton<IBloomFilter>(serviceKey, (serviceProvider, key) =>
            {
                var redisProvider = serviceProvider.GetKeyedService<IRedisProvider>(key);
                var bloomFilterOptions = serviceProvider.GetKeyedService<BloomFilterOptions>(key);
                return new RedisBloomFilter(redisProvider, bloomFilterOptions);
            });
        }
    }
}