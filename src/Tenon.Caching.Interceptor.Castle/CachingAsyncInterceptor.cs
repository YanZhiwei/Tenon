using System.Reflection;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using Tenon.Caching.Abstractions;
using Tenon.Caching.Interceptor.Castle.Attributes;
using Tenon.Caching.Interceptor.Castle.Configurations;
using Tenon.Caching.Interceptor.Castle.Exceptions;
using Tenon.Caching.Interceptor.Castle.Models;

namespace Tenon.Caching.Interceptor.Castle;

public sealed class CachingAsyncInterceptor(
    ICacheProvider cacheProvider,
    ICacheKeyGenerator cacheKeyBuilder,
    CachingInterceptorOptions options,
    ILogger<CachingAsyncInterceptor> logger)
    : IAsyncInterceptor
{
    private readonly ICacheProvider _cacheProvider =
        cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));

    private readonly ICacheKeyGenerator _keyGenerator =
        cacheKeyBuilder ?? throw new ArgumentNullException(nameof(cacheKeyBuilder));

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

        CachingAblIntercept(invocation, metaData);
        CachingEvictIntercept(invocation, metaData);
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

    private void CachingEvictIntercept(IInvocation invocation, InvocationMetadata metaData)
    {
        if (metaData.Attribute is CachingEvictAttribute attribute)
        {
            var interceptName = nameof(CachingEvictAttribute);
            var needRemovedKeys = GetNeedRemovedKeys(attribute, metaData).ToArray();

            _logger.LogDebug($"[{interceptName}] cacheKeys:{string.Join(",", needRemovedKeys)} interceptor starting.");
            try
            {
                InvocationProceed(invocation);
                if (needRemovedKeys?.Any() ?? false)
                {
                    var result = _cacheProvider.RemoveAll(needRemovedKeys);
                    _logger.LogDebug(
                        $"[{interceptName}] cacheKeys:{string.Join(",", needRemovedKeys)} remove keys:{result} succeeded.");
                }
            }
            catch (InvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
            catch (Exception ex)
            {
                if (!attribute.IsHighAvailability)
                    throw;
                _logger.LogError(ex, $"[{interceptName}] intercept failed.");
            }
        }
    }

    private void InvocationProceed(IInvocation invocation)
    {
        try
        {
            invocation.Proceed();
        }
        catch (Exception ex)
        {
            throw new InvocationException("invocation failed", ex);
        }
    }

    private IEnumerable<string> GetNeedRemovedKeys(CachingEvictAttribute attribute, InvocationMetadata metaData)
    {
        var needRemovedKeys = new HashSet<string>(StringComparer.Ordinal);
        if (!string.IsNullOrEmpty(attribute.CacheKey)) needRemovedKeys.Add(attribute.CacheKey);
        if (attribute.CacheKeys?.Any() ?? false) needRemovedKeys.UnionWith(attribute.CacheKeys);
        if (string.IsNullOrWhiteSpace(attribute.CacheKeyPrefix)) return needRemovedKeys;
        var cacheKeys =
            _keyGenerator.GetCacheKeys(metaData.MethodInfo, metaData.Arguments, attribute.CacheKeyPrefix);
        needRemovedKeys.UnionWith(cacheKeys);
        return needRemovedKeys;
    }

    private void CachingAblIntercept(IInvocation invocation, InvocationMetadata metaData)
    {
        if (metaData.Attribute is CachingAblAttribute attribute)
        {
            var interceptName = nameof(CachingAblIntercept);
            var cacheKey = string.IsNullOrEmpty(attribute.CacheKey)
                ? _keyGenerator.GetCacheKey(metaData.MethodInfo, metaData.Arguments, attribute.CacheKeyPrefix)
                : attribute.CacheKey;
            _logger.LogDebug($"[{interceptName}] cacheKey:{cacheKey} interceptor starting.");
            try
            {
                var cacheValue = _cacheProvider.Get<object>(cacheKey);
                if (cacheValue.HasValue)
                {
                    invocation.ReturnValue = cacheValue.Value;
                    _logger.LogDebug($"[{interceptName}] cacheKey:{cacheKey} hit cache succeeded");
                }
                else
                {
                    InvocationProceed(invocation);
                    if (invocation.ReturnValue != null)
                    {
                        _cacheProvider.Set(cacheKey, invocation.ReturnValue,
                            TimeSpan.FromSeconds(attribute.ExpirationInSec));
                        _logger.LogDebug($"[{interceptName}] cacheKey:{cacheKey} set cache succeeded");
                    }
                }
            }
            catch (InvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
            catch (Exception ex)
            {
                if (!attribute.IsHighAvailability)
                    throw;
                _logger.LogError(ex, $"[{interceptName}] intercept failed.");
            }
        }
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