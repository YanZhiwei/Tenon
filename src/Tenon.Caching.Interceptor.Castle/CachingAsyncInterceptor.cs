using Microsoft.Extensions.Logging;
using Tenon.Caching.Abstractions;
using Tenon.Infra.Castle.Attributes;
using Tenon.Infra.Castle.Interceptors;
using Tenon.Infra.Castle.Models;

namespace Tenon.Caching.Interceptor.Castle;

public sealed class CachingAsyncInterceptor : InterceptorBase
{
    private readonly ICacheProvider _cacheProvider;

    private readonly ILogger<CachingAsyncInterceptor>? _logger;

    public CachingAsyncInterceptor(ICacheProvider cacheProvider, ILogger<CachingAsyncInterceptor>? logger)
    {
        _cacheProvider = cacheProvider;
        _logger = logger;
    }

    protected override Task InterceptedAsync(InterceptAttribute attribute, InterceptMetadata metadata, Exception? ex)
    {
        throw new NotImplementedException();
    }

    protected override void Intercepted(InterceptAttribute attribute, InterceptMetadata metadata, Exception? ex)
    {
        throw new NotImplementedException();
    }
}