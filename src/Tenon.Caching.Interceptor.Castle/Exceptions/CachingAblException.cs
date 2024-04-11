namespace Tenon.Caching.Interceptor.Castle.Exceptions;

public sealed class CachingAblException(string message, string cacheKey, Exception innerException)
    : Exception(message, innerException)
{
    public string CacheKey = cacheKey ?? throw new ArgumentNullException(nameof(cacheKey));
}