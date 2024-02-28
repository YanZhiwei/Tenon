using Microsoft.Extensions.Configuration;
using Tenon.BloomFilter.Configurations;
using Tenon.BloomFilter.RedisStackExchange.Extensions;

namespace Tenon.BloomFilter.RedisStackExchange.Configurations;

public static class RedisStackExchangeBloomFilterOptions
{
    public static BloomFilterOptions UseRedisStackExchange(this BloomFilterOptions options,
        IConfigurationSection redisSection)
    {
        options.RegisterExtension(new BloomFilterOptionsExtension(redisSection, options));
        return options;
    }
}