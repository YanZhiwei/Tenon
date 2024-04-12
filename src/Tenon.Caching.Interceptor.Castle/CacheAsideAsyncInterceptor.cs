using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using Tenon.Caching.Abstractions;
using Tenon.Caching.Interceptor.Castle.Attributes;
using Tenon.Caching.Interceptor.Castle.Configurations;
using Tenon.Caching.Interceptor.Castle.Exceptions;
using Tenon.Caching.Interceptor.Castle.Extensions;
using Tenon.Caching.Interceptor.Castle.Models;

namespace Tenon.Caching.Interceptor.Castle;

/// <summary>
///     Cache Aside + 延时双删 + 失败补偿
/// </summary>
/// <param name="cacheProvider"></param>
/// <param name="cacheKeyBuilder"></param>
/// <param name="logger"></param>
public sealed class CacheAsideAsyncInterceptor(
    ICacheProvider cacheProvider,
    ICacheKeyGenerator cacheKeyBuilder,
    CacheAsideInterceptorOptions options,
    ILogger<CacheAsideAsyncInterceptor> logger)
    : IAsyncInterceptor
{
    private readonly ICacheProvider _cacheProvider =
        cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));

    private readonly ICacheKeyGenerator _keyGenerator =
        cacheKeyBuilder ?? throw new ArgumentNullException(nameof(cacheKeyBuilder));

    private readonly ILogger<CacheAsideAsyncInterceptor> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly CacheAsideInterceptorOptions
        _options = options ?? throw new ArgumentNullException(nameof(options));

    /// <summary>
    ///     同步拦截器
    /// </summary>
    /// <param name="invocation">IInvocation</param>
    public void InterceptSynchronous(IInvocation invocation)
    {
        var metaData = invocation.GetMetadata();
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
        var metaData = invocation.GetMetadata();
        if (metaData.Attribute == null)
        {
            invocation.Proceed();
            invocation.ReturnValue = (Task)invocation.ReturnValue;
            return;
        }

        invocation.ReturnValue = CachingInterceptAsync(invocation, metaData);
    }

    /// <summary>
    ///     异步拦截器 有返回值
    /// </summary>
    /// <typeparam name="TResult">TResult</typeparam>
    /// <param name="invocation">IInvocation</param>
    public void InterceptAsynchronous<TResult>(IInvocation invocation)
    {
        var metaData = invocation.GetMetadata();
        invocation.ReturnValue = CachingInterceptAsync<TResult>(metaData, invocation);
    }

    private async Task<TResult> CachingInterceptAsync<TResult>(InvocationMetadata metaData, IInvocation invocation)
    {
        TResult result;
        switch (metaData.Attribute)
        {
            case CachingAblAttribute attribute:
                result = await CachingAblInterceptAsync<TResult>(metaData, invocation, attribute);
                break;
            case CachingEvictAttribute evictAttribute:
                result = await CachingEvictInterceptAsync<TResult>(metaData, invocation, evictAttribute);
                break;
            default:
            {
                invocation.Proceed();
                var task = (Task<TResult>)invocation.ReturnValue;
                result = await task;
                break;
            }
        }

        return result;
    }

    /// <summary>
    ///     延时双删
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="metaData"></param>
    /// <param name="invocation"></param>
    /// <param name="evictAttribute"></param>
    /// <returns></returns>
    private async Task<TResult> CachingEvictInterceptAsync<TResult>(InvocationMetadata metaData, IInvocation invocation,
        CachingEvictAttribute evictAttribute)
    {
        var interceptName = evictAttribute.GetInterceptName();
        TResult result = default;
        var needRemovedKeys = GetNeedRemovedKeys(evictAttribute, metaData).ToArray();
        try
        {
            _logger.LogDebug($"[{interceptName}] cacheKeys:{string.Join(",", needRemovedKeys)} interceptor starting.");
            await DeleteCachingAsync(interceptName, needRemovedKeys, false);
            invocation.Proceed();
            var task = (Task<TResult>)invocation.ReturnValue;
            result = await task;
            await Task.Delay(_options.DelayedDelete);
            await DeleteCachingAsync(interceptName, needRemovedKeys);
        }
        catch (CachingEvictException ex)
        {
            CachingEvictFailedQueue.Instance.Enqueue(needRemovedKeys);
            if (!evictAttribute.IsHighAvailability)
                throw ex.InnerException ?? ex;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[{interceptName}] intercept failed.");
            if (!evictAttribute.IsHighAvailability)
                throw;
        }

        return result;
    }

    private async Task<TResult> CachingAblInterceptAsync<TResult>(InvocationMetadata metaData, IInvocation invocation,
        CachingAblAttribute attribute)
    {
        var interceptName = attribute.GetInterceptName();
        TResult result = default;
        try
        {
            var cacheKey = GetCachingAblKey(metaData, attribute);
            _logger.LogDebug($"[{interceptName}] cacheKey:{cacheKey} interceptor starting.");
            var cacheValue = await GetCacheValueAsync<TResult>(interceptName, cacheKey, !attribute.IsHighAvailability);
            if (cacheValue.HasValue)
            {
                result = cacheValue.Value;
                _logger.LogDebug($"[{interceptName}] cacheKey:{cacheKey} hit cache succeeded");
            }
            else
            {
                invocation.Proceed();
                result = await SetCacheAsync<TResult>(interceptName, cacheKey, invocation, attribute);
            }
        }
        catch (CachingAblException ex)
        {
            if (!attribute.IsHighAvailability)
                throw ex.InnerException ?? ex;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[{interceptName}] intercept failed.");
            if (!attribute.IsHighAvailability)
                throw;
        }

        return result;
    }

    private async Task<TResult> SetCacheAsync<TResult>(string interceptName, string cacheKey, IInvocation invocation,
        CachingAblAttribute attribute)
    {
        TResult result = default;
        try
        {
            var task = (Task<TResult>)invocation.ReturnValue;
            result = await task;
            if (result != null)
            {
                await _cacheProvider.SetAsync<TResult>(cacheKey, result,
                    TimeSpan.FromSeconds(attribute.ExpirationInSec));
                _logger.LogDebug($"[{interceptName}] cacheKey:{cacheKey} set cache succeeded");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"[{interceptName}] set cache key:{cacheKey} failed.");
            if (!attribute.IsHighAvailability)
                throw new CachingAblException(ex.Message, cacheKey, ex.InnerException ?? ex);
        }

        return result;
    }


    private async Task CachingInterceptAsync(IInvocation invocation, InvocationMetadata metaData)
    {
        if (metaData.Attribute is CachingAblAttribute)
        {
            invocation.Proceed();
            await (Task)invocation.ReturnValue;
        }

        if (metaData.Attribute is CachingEvictAttribute evictAttribute)
        {
            var interceptName = evictAttribute.GetInterceptName();
            var needRemovedKeys = GetNeedRemovedKeys(evictAttribute, metaData).ToArray();
            _logger.LogDebug($"[{interceptName}] cacheKeys:{string.Join(",", needRemovedKeys)} interceptor starting.");

            try
            {
                await DeleteCachingAsync(interceptName, needRemovedKeys, false);
                invocation.Proceed();
                await Task.Delay(_options.DelayedDelete);
                await DeleteCachingAsync(interceptName, needRemovedKeys);
            }
            catch (CachingEvictException ex)
            {
                CachingEvictFailedQueue.Instance.Enqueue(needRemovedKeys);
                if (!evictAttribute.IsHighAvailability)
                    throw ex.InnerException ?? ex;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{interceptName}] intercept failed.");
                if (!evictAttribute.IsHighAvailability)
                    throw;
            }
        }
    }

    private async Task DeleteCachingAsync(string interceptName, string[] needRemovedKeys, bool throwException = true)
    {
        if (needRemovedKeys?.Any() ?? false)
            try
            {
                var result = await _cacheProvider.RemoveAllAsync(needRemovedKeys);
                _logger.LogDebug(
                    $"[{interceptName}] delete cache keys:{string.Join(",", needRemovedKeys)} result:{result}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"[{interceptName}] delete cache keys:{string.Join(",", needRemovedKeys)} failed.");
                if (throwException)
                    throw new CachingEvictException(ex.Message, needRemovedKeys, ex.InnerException ?? ex);
            }
    }

    private void CachingEvictIntercept(IInvocation invocation, InvocationMetadata metaData)
    {
        if (metaData.Attribute is CachingEvictAttribute attribute)
        {
            var interceptName = attribute.GetInterceptName();
            var needRemovedKeys = GetNeedRemovedKeys(attribute, metaData).ToArray();

            _logger.LogDebug($"[{interceptName}] cacheKeys:{string.Join(",", needRemovedKeys)} interceptor starting.");
            try
            {
                DeleteCaching(interceptName, needRemovedKeys, false);
                invocation.Proceed();
                Thread.Sleep(_options.DelayedDelete);
                DeleteCaching(interceptName, needRemovedKeys);
            }
            catch (CachingEvictException ex)
            {
                CachingEvictFailedQueue.Instance.Enqueue(needRemovedKeys);
                if (!attribute.IsHighAvailability)
                    throw ex.InnerException ?? ex;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{interceptName}] intercept failed.");
                if (!attribute.IsHighAvailability)
                    throw;
            }
        }
    }


    private void DeleteCaching(string interceptName, string[] needRemovedKeys, bool throwException = true)
    {
        if (needRemovedKeys?.Any() ?? false)
            try
            {
                var result = _cacheProvider.RemoveAll(needRemovedKeys);
                _logger.LogDebug(
                    $"[{interceptName}] delete cache keys:{string.Join(",", needRemovedKeys)} result:{result}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"[{interceptName}] delete cache keys:{string.Join(",", needRemovedKeys)} failed.");
                if (throwException)
                    throw new CachingEvictException(ex.Message, needRemovedKeys, ex.InnerException ?? ex);
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
            var interceptName = attribute.GetInterceptName();
            var cacheKey = GetCachingAblKey(metaData, attribute);
            _logger.LogDebug($"[{interceptName}] cacheKey:{cacheKey} interceptor starting.");
            try
            {
                var cacheValue = GetCacheValue(interceptName, cacheKey, !attribute.IsHighAvailability);
                if (cacheValue.HasValue)
                {
                    invocation.ReturnValue = cacheValue.Value;
                    _logger.LogDebug($"[{interceptName}] cacheKey:{cacheKey} hit cache succeeded");
                }
                else
                {
                    invocation.Proceed();
                    SetCache(interceptName, cacheKey, invocation, attribute);
                }
            }
            catch (CachingAblException ex)
            {
                if (!attribute.IsHighAvailability)
                    throw ex.InnerException ?? ex;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{interceptName}] intercept failed.");
                if (!attribute.IsHighAvailability)
                    throw;
            }
        }
    }

    private void SetCache(string interceptName, string cacheKey, IInvocation invocation, CachingAblAttribute attribute)
    {
        try
        {
            if (invocation.ReturnValue != null)
            {
                _cacheProvider.Set(cacheKey, invocation.ReturnValue,
                    TimeSpan.FromSeconds(attribute.ExpirationInSec));
                _logger.LogDebug($"[{interceptName}] cacheKey:{cacheKey} set cache succeeded");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"[{interceptName}] set cache key:{cacheKey} failed.");
            if (!attribute.IsHighAvailability)
                throw new CachingAblException(ex.Message, cacheKey, ex.InnerException ?? ex);
        }
    }

    private async Task<CacheValue<TResult>> GetCacheValueAsync<TResult>(string interceptName, string cacheKey,
        bool throwException = true)
    {
        try
        {
            var cacheValue = await _cacheProvider.GetAsync<TResult>(cacheKey);
            _logger.LogDebug(
                $"[{interceptName}] get cache key:{cacheKey} succeeded.");
            return cacheValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"[{interceptName}] get cache key:{cacheKey} failed.");
            if (throwException)
                throw new CachingAblException(ex.Message, cacheKey, ex.InnerException ?? ex);
            return CacheValue<TResult>.NoValue;
        }
    }

    private CacheValue<object> GetCacheValue(string interceptName, string cacheKey, bool throwException = true)
    {
        try
        {
            var cacheValue = _cacheProvider.Get<object>(cacheKey);
            _logger.LogDebug(
                $"[{interceptName}] get cache key:{cacheKey} succeeded.");
            return cacheValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"[{interceptName}] get cache key:{cacheKey} failed.");
            if (throwException)
                throw new CachingAblException(ex.Message, cacheKey, ex.InnerException ?? ex);
            return CacheValue<object>.NoValue;
        }
    }

    private string GetCachingAblKey(InvocationMetadata metaData, CachingAblAttribute attribute)
    {
        return string.IsNullOrEmpty(attribute.CacheKey)
            ? _keyGenerator.GetCacheKey(metaData.MethodInfo, metaData.Arguments, attribute.CacheKeyPrefix)
            : attribute.CacheKey;
    }
}