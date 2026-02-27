namespace Tenon.Caching.Interceptor.Castle.Configurations;

/// <summary>
/// 缓存旁路拦截器选项（如延时双删间隔）。
/// </summary>
public sealed class CacheAsideInterceptorOptions
{
    /// <summary>
    /// 延时双删的第二次删除前等待时间，默认 1 秒。
    /// </summary>
    public TimeSpan DelayedDelete { get; set; } = TimeSpan.FromSeconds(1);
}