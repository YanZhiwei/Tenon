namespace Tenon.Caching.Interceptor.Castle.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class CachingInterceptorAttribute : Attribute
{
    public string CacheKeyPrefix { get; set; } = string.Empty;

    public bool IsHighAvailability { get; set; } = true;

    public string CacheKey { get; set; } = string.Empty;
}