namespace Tenon.Caching.Interceptor.Castle.Attributes;

/// <summary>
/// 标记参与缓存键生成的参数；未标记时默认使用全部参数。
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
public sealed class CachingParameterAttribute : CachingInterceptorAttribute
{
}