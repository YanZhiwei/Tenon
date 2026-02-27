namespace Tenon.Caching.Interceptor.Castle.Attributes;

/// <summary>
/// Cache Aside 读缓存/写缓存注解：先查缓存，未命中再执行方法并回写，可指定过期秒数。
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = true)]
public sealed class CachingAblAttribute : CachingInterceptorAttribute
{
    /// <summary>缓存过期时间（秒），默认 30。</summary>
    public int ExpirationInSec { get; set; } = 30;
}