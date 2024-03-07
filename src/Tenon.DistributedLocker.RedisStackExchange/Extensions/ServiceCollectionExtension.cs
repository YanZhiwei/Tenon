using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.DistributedLocker.Abstractions;
using Tenon.DistributedLocker.Abstractions.Configurations;
using Tenon.DistributedLocker.Redis;
using Tenon.Infra.Redis;
using Tenon.Infra.Redis.Configurations;
using Tenon.Infra.Redis.StackExchangeProvider.Extensions;

namespace Tenon.DistributedLocker.RedisStackExchange.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddRedisStackExchangeDistributedLocker(this IServiceCollection services,
        IConfigurationSection redisSection, Action<DistributedLockerOptions>? setupAction = null)
    {
        if (redisSection == null)
            throw new ArgumentNullException(nameof(redisSection));
        var redisConfig = redisSection.Get<RedisOptions>();
        if (redisConfig == null)
            throw new ArgumentNullException(nameof(redisConfig));
        if (string.IsNullOrWhiteSpace(redisConfig.ConnectionString))
            throw new ArgumentNullException(nameof(redisConfig.ConnectionString));
        services.Configure<RedisOptions>(redisSection);
        var options = new DistributedLockerOptions();
        if (setupAction != null)
            setupAction(options);
        services.AddSingleton(options);
        services.AddRedisStackExchangeProvider(redisSection);
        services.TryAddSingleton<IDistributedLocker, RedisDistributedLocker>();
        return services;
    }

    public static IServiceCollection AddKeyedRedisStackExchangeDistributedLocker(this IServiceCollection services,
        string serviceKey, IConfigurationSection redisSection, Action<DistributedLockerOptions>? setupAction = null)
    {
        if (string.IsNullOrEmpty(serviceKey))
            throw new ArgumentNullException(nameof(serviceKey));
        if (redisSection == null)
            throw new ArgumentNullException(nameof(redisSection));
        var redisConfig = redisSection.Get<RedisOptions>();
        if (redisConfig == null)
            throw new ArgumentNullException(nameof(redisConfig));
        if (string.IsNullOrWhiteSpace(redisConfig.ConnectionString))
            throw new ArgumentNullException(nameof(redisConfig.ConnectionString));
        services.AddKeyedRedisStackExchangeProvider(serviceKey, redisSection);
        var options = new DistributedLockerOptions();
        if (setupAction != null)
            setupAction(options);
        services.AddSingleton(options);
        services.TryAddKeyedSingleton<IDistributedLocker>(serviceKey,
            (serviceProvider, key) => new RedisDistributedLocker(serviceProvider.GetKeyedService<IRedisProvider>(key),
                options));
        return services;
    }
}