using Microsoft.Extensions.Configuration;
using Tenon.Caching.Configurations;
using Tenon.Caching.RedisStackExchange.Extensions;

namespace Tenon.Caching.RedisStackExchange.Configurations;

public static class RedisStackExchangeCachingOptions
{
    public static CachingOptions UseRedisStackExchange(this CachingOptions options,
        IConfigurationSection redisCacheSection)
    {
        options.RegisterExtension(new CachingOptionsExtension(redisCacheSection));
        return options;
    }

    public static CachingOptions UseKeyedRedisStackExchange(this CachingOptions options, string serviceKey,
        IConfigurationSection redisCacheSection)
    {
        if (string.IsNullOrWhiteSpace(serviceKey))
            throw new ArgumentNullException(nameof(serviceKey));
        options.RegisterExtension(new CachingOptionsExtension(redisCacheSection, serviceKey));
        return options;
    }
}