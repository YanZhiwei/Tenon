namespace Tenon.Caching.Interceptor.Castle.Exceptions;

/// <summary>
/// 缓存读写（Cache Aside）失败时抛出，携带相关缓存键。
/// </summary>
public sealed class CachingAblException(string message, string cacheKey, Exception innerException)
    : Exception(message, innerException)
{
    /// <summary>
    /// 发生异常时的缓存键。
    /// </summary>
    public string CacheKey { get; } = cacheKey ?? throw new ArgumentNullException(nameof(cacheKey));
}