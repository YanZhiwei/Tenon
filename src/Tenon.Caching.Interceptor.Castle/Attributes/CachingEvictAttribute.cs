namespace Tenon.Caching.Interceptor.Castle.Attributes;

public sealed class CachingEvictAttribute : CachingInterceptorAttribute
{
    public string[] CacheKeys { get; set; } = new string[] { };
}