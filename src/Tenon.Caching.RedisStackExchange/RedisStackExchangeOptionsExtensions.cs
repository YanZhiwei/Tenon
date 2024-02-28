using Microsoft.Extensions.Configuration;
using Tenon.Caching.Configurations;
using Tenon.Caching.RedisStackExchange.Extensions;

namespace Tenon.Caching.RedisStackExchange;

public static class RedisStackExchangeOptionsExtensions
{
    public static CachingOptions UseRedisStackExchange(this CachingOptions options,
        IConfigurationSection redisCacheSection)
    {
        if (redisCacheSection == null)
            throw new ArgumentNullException(nameof(redisCacheSection));
        options.RegisterExtension(new RedisStackExchangeOptionsExtension());
        return options;
    }
}