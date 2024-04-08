using Microsoft.Extensions.Logging;
using Tenon.Caching.Abstractions;
using Tenon.Caching.Interceptor.Castle.Attributes;
using Tenon.Caching.Interceptor.Castle.Configurations;
using Tenon.Infra.Castle.Attributes;
using Tenon.Infra.Castle.Interceptors;
using Tenon.Infra.Castle.Models;

namespace Tenon.Caching.Interceptor.Castle;

public sealed class CachingAsyncInterceptor(
    ICacheProvider cacheProvider,
    ICacheKeyBuilder cacheKeyBuilder,
    CachingInterceptorOptions options,
    ILogger<CachingAsyncInterceptor> logger)
    : InterceptorBase
{
    private readonly ICacheKeyBuilder _cacheKeyBuilder =
        cacheKeyBuilder ?? throw new ArgumentNullException(nameof(cacheKeyBuilder));

    private readonly ICacheProvider _cacheProvider =
        cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));

    private readonly ILogger<CachingAsyncInterceptor> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly CachingInterceptorOptions _options = options ?? throw new ArgumentNullException(nameof(options));

    protected override async Task InterceptedAsync(InterceptAttribute attribute, InterceptMetadata metadata,
        Exception? ex)
    {
        if (attribute is CachingEvictAttribute cachingEvict)
        {
            var needRemovedKeys = new HashSet<string>();
            if (cachingEvict.CacheKeys.Any())
                needRemovedKeys.UnionWith(needRemovedKeys);
            if (!string.IsNullOrWhiteSpace(cachingEvict.CacheKeyPrefix))
            {
                var cacheKeys = _cacheKeyBuilder.GetCacheKeys(metadata.MethodInfo, metadata.Arguments,
                    cachingEvict.CacheKeyPrefix);
                if (cacheKeys?.Any() ?? false)
                    needRemovedKeys.UnionWith(needRemovedKeys);
            }

            if (!string.IsNullOrEmpty(cachingEvict.CacheKey))
                needRemovedKeys.Add(cachingEvict.CacheKey);

            await _cacheProvider.KeysExpireAsync(needRemovedKeys);
        }

        await Task.CompletedTask;
    }

    protected override void Intercepted(InterceptAttribute attribute, InterceptMetadata metadata, Exception? ex)
    {
        throw new NotImplementedException();
    }
}