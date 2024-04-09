namespace Tenon.Caching.Interceptor.Castle.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = true)]
public sealed class CachingAblAttribute : CachingInterceptorAttribute
{
    public int ExpirationInSec { get; set; } = 30;
}