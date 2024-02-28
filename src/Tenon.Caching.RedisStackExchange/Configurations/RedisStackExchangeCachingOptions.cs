using Microsoft.Extensions.Configuration;
using Tenon.Caching.Abstractions.Configurations;
using Tenon.Caching.RedisStackExchange.Extensions;

namespace Tenon.Caching.RedisStackExchange.Configurations;

public static class RedisStackExchangeCachingOptions
{
    public static CachingOptions UseRedisStackExchange(this CachingOptions options,
        IConfigurationSection redisCacheSection)
    {
        options.RegisterExtension(new CachingOptionsExtension(redisCacheSection, options));
        return options;
    }

    public static CachingOptions UseSystemTextJsonSerializer(this CachingOptions options)
    {
        options.RegisterExtension(new SerializerOptionsExtension(true, options));
        return options;
    }
}