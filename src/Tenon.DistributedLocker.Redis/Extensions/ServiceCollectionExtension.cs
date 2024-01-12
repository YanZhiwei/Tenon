using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.Redis;

namespace Tenon.DistributedLocker.Redis.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddRedisDistributedLocker<TRedisProvider>(this IServiceCollection services)
        where TRedisProvider : class, IRedisProvider
    {
        services.TryAddSingleton<IRedisProvider, TRedisProvider>();
        services.TryAddSingleton<IDistributedLocker, RedisDistributedLocker>();
        return services;
    }
}