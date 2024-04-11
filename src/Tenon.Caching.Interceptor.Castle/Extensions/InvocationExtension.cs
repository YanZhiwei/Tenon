using System.Reflection;
using Castle.DynamicProxy;
using Tenon.Caching.Interceptor.Castle.Attributes;
using Tenon.Caching.Interceptor.Castle.Models;

namespace Tenon.Caching.Interceptor.Castle.Extensions;

internal static class InvocationExtension
{
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