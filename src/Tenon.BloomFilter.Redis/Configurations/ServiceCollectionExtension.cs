using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.Redis;

namespace Tenon.BloomFilter.Redis.Configurations;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddRedisBloomFilter(this IServiceCollection services)
    {
        services.TryAddSingleton<IBloomFilter, RedisBloomFilter>();
        return services;
    }

    public static IServiceCollection AddKeyedRedisBloomFilter(this IServiceCollection services, string serviceKey,
        bool requiredKeyedService = false)
    {
        if (string.IsNullOrWhiteSpace(serviceKey))
            throw new ArgumentNullException(nameof(serviceKey));
        services.TryAddKeyedScoped<IBloomFilter>(serviceKey, (serviceProvider, key) =>
        {
            var redisProvider = serviceProvider.GetKeyedService<IRedisProvider>(key);
            if (!requiredKeyedService && redisProvider == null)
                redisProvider = serviceProvider.GetService<IRedisProvider>();
            return new RedisBloomFilter(redisProvider);
        });
        return services;
    }
}