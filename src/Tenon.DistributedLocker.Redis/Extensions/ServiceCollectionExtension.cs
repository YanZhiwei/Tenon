using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.DistributedLocker.Configurations;
using Tenon.Redis;

namespace Tenon.DistributedLocker.Redis.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddRedisDistributedLocker<TRedisProvider>(this IServiceCollection services,
        IConfigurationSection? distributedLockerSection = null)
        where TRedisProvider : class, IRedisProvider
    {
        var distributedLockerConfig = distributedLockerSection?.Get<DistributedLockerOptions>();
        if (distributedLockerConfig != null && distributedLockerSection != null)
            services.Configure<DistributedLockerOptions>(distributedLockerSection);
        services.TryAddSingleton<IRedisProvider, TRedisProvider>();
        services.TryAddSingleton<IDistributedLocker, RedisDistributedLocker>();
        return services;
    }
}