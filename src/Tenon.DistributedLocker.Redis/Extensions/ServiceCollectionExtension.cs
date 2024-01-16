using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.DistributedLocker.Redis.Configurations;
using Tenon.Redis;

namespace Tenon.DistributedLocker.Redis.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddRedisDistributedLocker(this IServiceCollection services,
        IConfigurationSection? distributedLockerSection = null)
    {
        var distributedLockerConfig = distributedLockerSection?.Get<RedisDistributedLockerOptions>();
        if (distributedLockerConfig != null && distributedLockerSection != null)
            services.Configure<RedisDistributedLockerOptions>(distributedLockerSection);
        services.TryAddSingleton<IDistributedLocker, RedisDistributedLocker>();
        return services;
    }

    public static IServiceCollection AddKeyedRedisDistributedLocker(this IServiceCollection services,
        string serviceKey, IConfigurationSection? distributedLockerSection = null)
    {
        if (string.IsNullOrEmpty(serviceKey))
            throw new ArgumentNullException(nameof(serviceKey));
        var distributedLockerConfig = distributedLockerSection?.Get<RedisDistributedLockerOptions>();
        services.TryAddKeyedSingleton<IDistributedLocker>(serviceKey,
            (serviceProvider, key) => new RedisDistributedLocker(serviceProvider.GetKeyedService<IRedisProvider>(key),
                distributedLockerConfig));
        return services;
    }
}