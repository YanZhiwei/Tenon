namespace Tenon.Caching.Interceptor.Castle.Attributes;

/// <summary>
/// 缓存失效注解：方法执行前后按配置删除指定 key（延时双删），失败可入队补偿。
/// </summary>
public sealed class CachingEvictAttribute : CachingInterceptorAttribute
{
    /// <summary>要失效的缓存键或键模板，可与方法参数组合。</summary>
    public string[] CacheKeys { get; set; } = Array.Empty<string>();
}