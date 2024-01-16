using Tenon.Redis.Configurations;

namespace Tenon.Caching.Redis.Configurations;

public sealed class RedisCachingOptions
{
    public int MaxRandomSecond { get; set; }

    public RedisOptions Redis { get; set; }
}
