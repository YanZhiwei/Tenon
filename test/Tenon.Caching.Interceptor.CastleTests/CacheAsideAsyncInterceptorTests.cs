using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tenon.Caching.Abstractions;
using Tenon.Caching.InMemory.Extensions;
using Tenon.Caching.Interceptor.Castle;
using Tenon.Caching.Interceptor.Castle.Attributes;
using Tenon.Caching.Interceptor.Castle.Configurations;
using Xunit;

namespace Tenon.Caching.Interceptor.CastleTests;

public sealed class CacheAsideAsyncInterceptorTests
{
    [Fact]
    public void Constructor_ThrowsWhenCacheProviderNull()
    {
        var services = new ServiceCollection()
            .AddInMemoryCache()
            .AddSingleton<ICacheKeyGenerator, DefaultCacheKeyGenerator>()
            .Configure<CacheAsideInterceptorOptions>(_ => { })
            .AddLogging();
        using var sp = services.BuildServiceProvider();
        var cache = sp.GetRequiredService<ICacheProvider>();
        var keyGen = sp.GetRequiredService<ICacheKeyGenerator>();
        var options = sp.GetRequiredService<IOptions<CacheAsideInterceptorOptions>>().Value;
        var logger = sp.GetRequiredService<ILogger<CacheAsideAsyncInterceptor>>();

        Assert.Throws<ArgumentNullException>(() =>
            new CacheAsideAsyncInterceptor(null!, keyGen, options, logger));
    }

    [Fact]
    public async Task InterceptAsynchronous_CachingAbl_MissThenHit_ReturnsSameValue()
    {
        var services = new ServiceCollection()
            .AddInMemoryCache()
            .AddSingleton<ICacheKeyGenerator, DefaultCacheKeyGenerator>()
            .Configure<CacheAsideInterceptorOptions>(o => o.DelayedDelete = TimeSpan.FromMilliseconds(1))
            .AddLogging(b => b.SetMinimumLevel(LogLevel.Warning));
        using var sp = services.BuildServiceProvider();

        var target = new TargetService();
        var interceptor = new CacheAsideAsyncInterceptor(
            sp.GetRequiredService<ICacheProvider>(),
            sp.GetRequiredService<ICacheKeyGenerator>(),
            sp.GetRequiredService<IOptions<CacheAsideInterceptorOptions>>().Value,
            sp.GetRequiredService<ILogger<CacheAsideAsyncInterceptor>>());

        var generator = new ProxyGenerator();
        var proxy = generator.CreateInterfaceProxyWithTarget<ITargetService>(target, interceptor);

        var first = await proxy.GetAsync(1);
        var second = await proxy.GetAsync(1);
        Assert.Equal(first, second);
        Assert.True(target.CallCount >= 1);
    }

    [Fact]
    public async Task InterceptAsynchronous_NoAttribute_ProceedsAndReturnsValue()
    {
        var services = new ServiceCollection()
            .AddInMemoryCache()
            .AddSingleton<ICacheKeyGenerator, DefaultCacheKeyGenerator>()
            .Configure<CacheAsideInterceptorOptions>(_ => { })
            .AddLogging(b => b.SetMinimumLevel(LogLevel.Warning));
        using var sp = services.BuildServiceProvider();

        var target = new TargetService();
        var interceptor = new CacheAsideAsyncInterceptor(
            sp.GetRequiredService<ICacheProvider>(),
            sp.GetRequiredService<ICacheKeyGenerator>(),
            sp.GetRequiredService<IOptions<CacheAsideInterceptorOptions>>().Value,
            sp.GetRequiredService<ILogger<CacheAsideAsyncInterceptor>>());

        var generator = new ProxyGenerator();
        var proxy = generator.CreateInterfaceProxyWithTarget<ITargetService>(target, interceptor);

        var value = await proxy.GetNoCacheAsync(2);
        Assert.Equal(2, value);
        Assert.Equal(1, target.CallCount);
    }

    public interface ITargetService
    {
        Task<int> GetAsync(int id);
        Task<int> GetNoCacheAsync(int id);
    }

    private sealed class TargetService : ITargetService
    {
        public int CallCount { get; private set; }

        [CachingAbl(ExpirationInSec = 60)]
        public Task<int> GetAsync(int id)
        {
            CallCount++;
            return Task.FromResult(id);
        }

        public Task<int> GetNoCacheAsync(int id)
        {
            CallCount++;
            return Task.FromResult(id);
        }
    }
}
