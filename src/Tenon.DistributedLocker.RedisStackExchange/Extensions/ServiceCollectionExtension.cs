using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.DistributedLocker.Redis;
using Tenon.DistributedLocker.Redis.Configurations;
using Tenon.Redis;
using Tenon.Redis.StackExchangeProvider.Extensions;

namespace Tenon.DistributedLocker.RedisStackExchange.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddRedisStackExchangeDistributedLocker(this IServiceCollection services,
        IConfigurationSection distributedLockerSection)
    {
        if (distributedLockerSection == null)
            throw new ArgumentNullException(nameof(distributedLockerSection));
        var distributedLockerConfig = distributedLockerSection.Get<RedisDistributedLockerOptions>();
        if (distributedLockerConfig == null)
            throw new ArgumentException(nameof(distributedLockerSection));
        services.Configure<RedisDistributedLockerOptions>(distributedLockerSection);
        services.AddRedisStackExchangeProvider(
            distributedLockerSection.GetSection(nameof(RedisDistributedLockerOptions.Redis)));
        services.TryAddSingleton<IDistributedLocker, RedisDistributedLocker>();
        return services;
    }

    public static IServiceCollection AddKeyedRedisStackExchangeDistributedLocker(this IServiceCollection services,
        string serviceKey, IConfigurationSection distributedLockerSection)
    {
        if (string.IsNullOrEmpty(serviceKey))
            throw new ArgumentNullException(nameof(serviceKey));
        if (distributedLockerSection == null)
            throw new ArgumentNullException(nameof(distributedLockerSection));
        var distributedLockerConfig = distributedLockerSection.Get<RedisDistributedLockerOptions>();
        if (distributedLockerConfig == null)
            throw new ArgumentException(nameof(distributedLockerSection));
        services.AddKeyedRedisStackExchangeProvider(serviceKey,
            distributedLockerSection.GetSection(nameof(RedisDistributedLockerOptions.Redis)));
        services.TryAddKeyedSingleton<IDistributedLocker>(serviceKey,
            (serviceProvider, key) => new RedisDistributedLocker(serviceProvider.GetKeyedService<IRedisProvider>(key),
                distributedLockerConfig));
        return services;
    }
}