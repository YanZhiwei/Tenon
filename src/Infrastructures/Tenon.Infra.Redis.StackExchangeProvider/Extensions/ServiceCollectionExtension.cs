using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.Infra.Redis.Configurations;
using Tenon.Serialization.Abstractions;

namespace Tenon.Infra.Redis.StackExchangeProvider.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddRedisStackExchangeProvider<TSerializer>(this IServiceCollection services,
        IConfigurationSection redisSection) where TSerializer : class, ISerializer
    {
        var redisConfig = redisSection.Get<RedisOptions>();
        if (redisConfig == null)
            throw new NullReferenceException(nameof(redisConfig));

        services.Configure<RedisOptions>(redisSection);
        services.TryAddSingleton<ISerializer, TSerializer>();
        services.TryAddSingleton<IRedisProvider, Infra.Redis.StackExchangeProvider.StackExchangeProvider>();
        return services;
    }

    public static IServiceCollection AddRedisStackExchangeProvider(this IServiceCollection services,
        IConfigurationSection redisSection)
    {
        var redisConfig = redisSection.Get<RedisOptions>();
        if (redisConfig == null)
            throw new NullReferenceException(nameof(redisConfig));

        services.Configure<RedisOptions>(redisSection);
        services.TryAddSingleton<IRedisProvider, Infra.Redis.StackExchangeProvider.StackExchangeProvider>();
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
        services.TryAddKeyedSingleton<IRedisProvider>(serviceKey, (serviceProvider, key) =>
        {
            var serializer = serviceProvider.GetKeyedService<ISerializer>(key);
#pragma warning disable CS8604 // Possible null reference argument.
            return new Infra.Redis.StackExchangeProvider.StackExchangeProvider(redisConfig, serializer);
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
        services.TryAddKeyedSingleton<IRedisProvider>(serviceKey, (serviceProvider, key) =>
        {
            var serializer = serviceProvider.GetKeyedService<ISerializer>(key);
            return new StackExchangeProvider(redisConfig, serializer);
        });
        return services;
    }
}