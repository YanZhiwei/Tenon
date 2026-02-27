using System.Reflection;
using Castle.DynamicProxy;
using Tenon.Caching.Interceptor.Castle.Attributes;
using Tenon.Caching.Interceptor.Castle.Models;

namespace Tenon.Caching.Interceptor.Castle.Extensions;

/// <summary>
/// Castle 调用的扩展，用于提取缓存拦截所需元数据。
/// </summary>
internal static class InvocationExtension
{
    /// <summary>
    /// 从当前调用中提取方法、参数及缓存注解（若有）。
    /// </summary>
    /// <param name="invocation">Castle 拦截调用。</param>
    /// <returns>调用元数据。</returns>
    public static InvocationMetadata GetMetadata(this IInvocation invocation)
    {
        var methodInfo = invocation.Method ?? invocation.MethodInvocationTarget;
        var attribute = methodInfo.GetCustomAttribute<CachingInterceptorAttribute>();

        var metadata = new InvocationMetadata
        {
            Arguments = invocation.Arguments,
            Attribute = attribute,
            ClassName = methodInfo.DeclaringType?.FullName ?? string.Empty,
            MethodInfo = methodInfo
        };
        return metadata;
    }
}