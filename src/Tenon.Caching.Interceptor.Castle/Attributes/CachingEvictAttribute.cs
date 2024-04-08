using Tenon.Infra.Castle.Attributes;

namespace Tenon.Caching.Interceptor.Castle.Attributes;

public sealed class CachingEvictAttribute : InterceptAttribute
{
    public string CacheKeyPrefix { get; set; } = string.Empty;
    public string[] CacheKeys { get; set; } = new string[] { };

    public string CacheKey { get; set; } = string.Empty;
}