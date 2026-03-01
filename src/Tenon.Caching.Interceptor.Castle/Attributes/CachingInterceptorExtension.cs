namespace Tenon.Caching.Interceptor.Castle.Attributes;

/// <summary>
/// 缓存拦截器注解的扩展方法。
/// </summary>
internal static class CachingInterceptorExtension
{
    /// <summary>
    /// 获取拦截类型名称（如 CachingAbl、CachingEvict），用于日志等。
    /// </summary>
    /// <param name="interceptorAttribute">方法上的缓存拦截注解。</param>
    /// <returns>去掉 Attribute 后缀的简短名称。</returns>
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
