using Microsoft.Extensions.Configuration;
using Tenon.DistributedLocker.Abstractions.Configurations;
using Tenon.DistributedLocker.RedisStackExchange.Extensions;

namespace Tenon.DistributedLocker.RedisStackExchange.Configurations;

public static class RedisStackExchangeDistributedLockerOptions
{
    public static DistributedLockerOptions UseRedisStackExchange(this DistributedLockerOptions options,
        IConfigurationSection redisSection)
    {
        options.RegisterExtension(new DistributedLockerOptionsExtension(redisSection, options));
        return options;
    }
}