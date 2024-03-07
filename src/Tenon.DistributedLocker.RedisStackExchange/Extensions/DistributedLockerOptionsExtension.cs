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

internal class DistributedLockerOptionsExtension(IConfigurationSection redisSection, DistributedLockerOptions options)
    : IDistributedLockerOptionsExtension
{
    private readonly DistributedLockerOptions _options = options ?? throw new ArgumentNullException(nameof(options));

    private readonly IConfigurationSection _redisSection =
        redisSection ?? throw new ArgumentNullException(nameof(redisSection));

    public void AddServices(IServiceCollection services)
    {
        if (_redisSection == null)
            throw new ArgumentNullException(nameof(_redisSection));
        var redisConfig = _redisSection.Get<RedisOptions>();
        if (redisConfig == null)
            throw new ArgumentNullException(nameof(_redisSection));
        if (string.IsNullOrWhiteSpace(options.KeyedServiceKey))
        {
            services.AddRedisStackExchangeProvider(_redisSection);
            services.TryAddSingleton<IDistributedLocker, RedisDistributedLocker>();
        }
        else
        {
            var serviceKey = _options.KeyedServiceKey;
            services.AddKeyedRedisStackExchangeProvider(serviceKey, _redisSection);
            services.TryAddKeyedSingleton<IDistributedLocker>(serviceKey, (serviceProvider, key) =>
            {
                var redisProvider = serviceProvider.GetKeyedService<IRedisProvider>(key);
                var bloomFilterOptions = serviceProvider.GetKeyedService<DistributedLockerOptions>(key);
                return new RedisDistributedLocker(redisProvider, bloomFilterOptions);
            });
        }
    }
}