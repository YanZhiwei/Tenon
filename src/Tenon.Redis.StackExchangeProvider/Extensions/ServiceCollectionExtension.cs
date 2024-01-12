using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.Redis.StackExchangeProvider.Configurations;
using Tenon.Serialization;

namespace Tenon.Redis.StackExchangeProvider.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddRedisStackExchangeProvider<TSerializer>(this IServiceCollection services,
        IConfigurationSection redisSection) where TSerializer : class, ISerializer
    {
        var redisConfig = redisSection.Get<RedisOptions>();
        if (redisConfig == null)
            throw new NullReferenceException(nameof(redisConfig));
        services.Configure<RedisOptions>(redisSection);
        services.TryAddSingleton<RedisConnection>();
        services.TryAddSingleton<ISerializer, TSerializer>();
        services.TryAddSingleton<IRedisProvider, StackExchangeProvider>();
        return services;
    }
}