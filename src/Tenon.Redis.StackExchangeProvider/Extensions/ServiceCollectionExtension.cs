using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.Redis.StackExchangeProvider.Configurations;
using Tenon.Serialization;
using Tenon.Serialization.Json;

namespace Tenon.Redis.StackExchangeProvider.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddRedisStackExchangeProvider(this IServiceCollection services,
        IConfigurationSection redisSection)
    {
        var redisConfig = redisSection.Get<RedisOptions>();
        if (redisConfig == null)
            throw new NullReferenceException(nameof(redisConfig));
        services.Configure<RedisOptions>(redisSection);
        services.TryAddSingleton<RedisConnection>();
        services.TryAddSingleton<ISerializer, JsonSerializer>();
        services.TryAddSingleton<IRedisProvider, StackExchangeProvider>();
        return services;
    }
}