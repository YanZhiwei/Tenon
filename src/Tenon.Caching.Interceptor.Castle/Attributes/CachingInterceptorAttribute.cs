namespace Tenon.Caching.Interceptor.Castle.Attributes;

/// <summary>
/// 缓存拦截器注解基类，用于标记需走缓存逻辑的方法。
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class CachingInterceptorAttribute : Attribute
{
    /// <summary>缓存键前缀，会参与最终 key 的生成。</summary>
    public string CacheKeyPrefix { get; set; } = string.Empty;

    /// <summary>为 true 时异常不向外抛，仅记录；为 false 时抛出。</summary>
    public bool IsHighAvailability { get; set; } = true;

    /// <summary>指定缓存键（可选），与参数生成的 key 一起使用。</summary>
    public string CacheKey { get; set; } = string.Empty;
}