using Tenon.Caching.Interceptor.Castle.Attributes;

namespace Tenon.Caching.Interceptor.Castle.Extensions;

internal static class CachingInterceptorExtension
{
    public static string GetInterceptName(this CachingInterceptorAttribute interceptorAttribute)
    {
        return interceptorAttribute switch
        {
            null => string.Empty,
            CachingAblAttribute => nameof(CachingAblAttribute).Replace("Attribute", ""),
            CachingEvictAttribute => nameof(CachingEvictAttribute).Replace("Attribute", ""),
            _ => nameof(interceptorAttribute)
        };
    }
}