using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.Redis;

namespace Tenon.BloomFilter.Redis.Configurations;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddRedisBloomFilter<TRedisProvider>(this IServiceCollection services)
        where TRedisProvider : class, IRedisProvider
    {
        services.TryAddSingleton<IRedisProvider, TRedisProvider>();
        services.TryAddSingleton<IBloomFilter, RedisBloomFilter>();
        return services;
    }
}