using System.Reflection;

namespace Tenon.Caching.Interceptor.Castle.Models;

/// <summary>
/// 拦截调用元数据：方法、类名、参数及缓存注解。
/// </summary>
internal sealed class InvocationMetadata
{
    /// <summary>当前调用的方法。</summary>
    public MethodInfo MethodInfo { get; set; }

    /// <summary>声明该方法的类型全名。</summary>
    public string ClassName { get; set; }

    /// <summary>方法参数列表。</summary>
    public object[] Arguments { get; set; }

    /// <summary>方法上的缓存拦截注解（CachingAbl / CachingEvict），无则为 null。</summary>
    public Attribute? Attribute { get; set; }
}