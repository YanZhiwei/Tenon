namespace Tenon.Caching.Interceptor.Castle.Configurations;

public sealed class CacheAsideInterceptorOptions
{
    public TimeSpan DelayedDelete { get; set; } = TimeSpan.FromSeconds(1);
}