using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.Redis.Configurations;
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

    public static IServiceCollection AddRedisStackExchangeProvider(this IServiceCollection services,
        IConfigurationSection redisSection)
    {
        var redisConfig = redisSection.Get<RedisOptions>();
        if (redisConfig == null)
            throw new NullReferenceException(nameof(redisConfig));

        services.Configure<RedisOptions>(redisSection);
        services.TryAddSingleton<RedisConnection>();
        services.TryAddSingleton<IRedisProvider, StackExchangeProvider>();
        return services;
    }

    public static IServiceCollection AddKeyedRedisStackExchangeProvider<TSerializer>(this IServiceCollection services,
        string? serviceKey, IConfigurationSection redisSection) where TSerializer : class, ISerializer
    {
        if (string.IsNullOrEmpty(serviceKey))
            throw new ArgumentNullException(nameof(serviceKey));

        var redisConfig = redisSection.Get<RedisOptions>();
        if (redisConfig == null)
            throw new NullReferenceException(nameof(redisConfig));
        services.AddOptions();
        services.TryAddKeyedSingleton<ISerializer, TSerializer>(serviceKey);
        services.TryAddKeyedSingleton<RedisConnection>(serviceKey,
            (_, _) => new RedisConnection(redisConfig));
        services.TryAddKeyedSingleton<IRedisProvider>(serviceKey, (serviceProvider, key) =>
        {
            var redisConnection = serviceProvider.GetKeyedService<RedisConnection>(key);
            var serializer = serviceProvider.GetKeyedService<ISerializer>(key);
#pragma warning disable CS8604 // Possible null reference argument.
            return new StackExchangeProvider(redisConnection, serializer);
#pragma warning restore CS8604 // Possible null reference argument.
        });
        return services;
    }


    public static IServiceCollection AddKeyedRedisStackExchangeProvider(this IServiceCollection services,
        string? serviceKey, IConfigurationSection redisSection)
    {
        if (string.IsNullOrEmpty(serviceKey))
            throw new ArgumentNullException(nameof(serviceKey));

        var redisConfig = redisSection.Get<RedisOptions>();
        if (redisConfig == null)
            throw new NullReferenceException(nameof(redisConfig));
        services.AddOptions();
        services.TryAddKeyedSingleton<RedisConnection>(serviceKey,
            (_, _) => new RedisConnection(redisConfig));
        services.TryAddKeyedSingleton<IRedisProvider>(serviceKey, (serviceProvider, key) =>
        {
            var redisConnection = serviceProvider.GetKeyedService<RedisConnection>(key);
            var serializer = serviceProvider.GetKeyedService<ISerializer>(key);
#pragma warning disable CS8604 // Possible null reference argument.
            return new StackExchangeProvider(redisConnection, serializer);
#pragma warning restore CS8604 // Possible null reference argument.
        });
        return services;
    }
}