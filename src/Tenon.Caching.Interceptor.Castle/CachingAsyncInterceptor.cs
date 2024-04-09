using System.Reflection;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using Tenon.Caching.Abstractions;
using Tenon.Caching.Interceptor.Castle.Attributes;
using Tenon.Caching.Interceptor.Castle.Configurations;
using Tenon.Caching.Interceptor.Castle.Models;

namespace Tenon.Caching.Interceptor.Castle;

public sealed class CachingAsyncInterceptor(
    ICacheProvider cacheProvider,
    ICacheKeyBuilder cacheKeyBuilder,
    CachingInterceptorOptions options,
    ILogger<CachingAsyncInterceptor> logger)
    : IAsyncInterceptor
{
    private readonly ICacheKeyBuilder _cacheKeyBuilder =
        cacheKeyBuilder ?? throw new ArgumentNullException(nameof(cacheKeyBuilder));

    private readonly ICacheProvider _cacheProvider =
        cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));

    private readonly ILogger<CachingAsyncInterceptor> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly CachingInterceptorOptions _options = options ?? throw new ArgumentNullException(nameof(options));

    /// <summary>
    ///     同步拦截器
    /// </summary>
    /// <param name="invocation">IInvocation</param>
    public void InterceptSynchronous(IInvocation invocation)
    {
        var metaData = GetMetadata(invocation);
        if (metaData.Attribute == null)
        {
            invocation.Proceed();
            return;
        }

        CachingAblIntercept(metaData);
    }

    private void CachingAblIntercept(InvocationMetadata metaData)
    {
        if (metaData.Attribute is CachingAblAttribute attribute)
        {
            var cacheKey = string.IsNullOrEmpty(attribute.CacheKey)
                ? _cacheKeyBuilder.GetCacheKey(metaData.MethodInfo, metaData.Arguments, attribute.CacheKeyPrefix)
                : attribute.CacheKey;
            try
            {
                var cacheValue = _cacheProvider.Get<object>(cacheKey);
            }
            catch (Exception ex)
            {

            }
        }
    }

    /// <summary>
    ///     异步拦截器 无返回值
    /// </summary>
    /// <param name="invocation">IInvocation</param>
    public void InterceptAsynchronous(IInvocation invocation)
    {
    }

    /// <summary>
    ///     异步拦截器 有返回值
    /// </summary>
    /// <typeparam name="TResult">TResult</typeparam>
    /// <param name="invocation">IInvocation</param>
    public void InterceptAsynchronous<TResult>(IInvocation invocation)
    {
    }

    private InvocationMetadata GetMetadata(IInvocation invocation)
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