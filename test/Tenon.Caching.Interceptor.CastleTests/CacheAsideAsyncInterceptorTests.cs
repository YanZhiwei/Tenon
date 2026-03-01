using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tenon.Caching.Abstractions;
using Tenon.Caching.InMemory.Extensions;
using Tenon.Caching.Interceptor.Castle;
using Tenon.Caching.Interceptor.Castle.Attributes;
using Tenon.Caching.Interceptor.Castle.Extensions;
using Tenon.Caching.Interceptor.Castle.KeyGenerators;
using Tenon.Caching.Interceptor.Castle.Configurations;
using Xunit;

namespace Tenon.Caching.Interceptor.CastleTests;

public sealed class CacheAsideAsyncInterceptorTests
{
    [Fact]
    public void AddProxiesFromAssembly_WithInMemoryCache_RegistersAllDependencies()
    {
        var services = new ServiceCollection()
            .AddInMemoryCache()
            .AddProxiesFromAssembly(typeof(ITargetService).Assembly, o => o.DelayedDelete = TimeSpan.FromMilliseconds(1));
        using var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var proxy = scope.ServiceProvider.GetRequiredService<ITargetService>();
        Assert.NotNull(proxy);
    }

    [Fact]
    public void AddCachedProxyServices_WithInMemoryCache_RegistersAllDependencies()
    {
        var services = new ServiceCollection()
            .AddInMemoryCache()
            .AddCachedProxyServices<ITargetService, TargetService>(
                o => o.DelayedDelete = TimeSpan.FromMilliseconds(1));
        using var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var proxy = scope.ServiceProvider.GetRequiredService<ITargetService>();
        Assert.NotNull(proxy);
    }

    [Fact]
    public void Constructor_ThrowsWhenCacheProviderNull()
    {
        var services = new ServiceCollection()
            .AddInMemoryCache()
            .AddCachedProxyServices<ITargetService, TargetService>();
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
        var target = new TargetService();
        var services = new ServiceCollection()
            .AddInMemoryCache()
            .AddScoped<TargetService>(_ => target)
            .AddProxiesFromAssembly(typeof(ITargetService).Assembly, o => o.DelayedDelete = TimeSpan.FromMilliseconds(1));
        using var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var proxy = scope.ServiceProvider.GetRequiredService<ITargetService>();

        var first = await proxy.GetAsync(1);
        var second = await proxy.GetAsync(1);
        Assert.Equal(first, second);
        Assert.True(target.CallCount >= 1);
    }

    [Fact]
    public async Task AddProxiesFromAssembly_ResolvesProxy_CachingWorks()
    {
        var target = new TargetService();
        var services = new ServiceCollection()
            .AddInMemoryCache()
            .AddScoped<TargetService>(_ => target)
            .AddProxiesFromAssembly(typeof(ITargetService).Assembly, o => o.DelayedDelete = TimeSpan.FromMilliseconds(1));
        using var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var proxy = scope.ServiceProvider.GetRequiredService<ITargetService>();

        var first = await proxy.GetAsync(1);
        var second = await proxy.GetAsync(1);
        Assert.Equal(first, second);
        Assert.True(target.CallCount >= 1);

        var value = await proxy.GetNoCacheAsync(2);
        Assert.Equal(2, value);
    }

    [Fact]
    public async Task AddCachedProxyServices_ResolvesProxy_CachingWorksWithoutManualProxyCreation()
    {
        var target = new TargetService();
        var services = new ServiceCollection()
            .AddInMemoryCache()
            .AddScoped<TargetService>(_ => target)
            .AddCachedProxyServices<ITargetService, TargetService>(
                o => o.DelayedDelete = TimeSpan.FromMilliseconds(1));
        using var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var proxy = scope.ServiceProvider.GetRequiredService<ITargetService>();

        var first = await proxy.GetAsync(1);
        var second = await proxy.GetAsync(1);
        Assert.Equal(first, second);
        Assert.True(target.CallCount >= 1);

        var value = await proxy.GetNoCacheAsync(2);
        Assert.Equal(2, value);
    }

    [Fact]
    public async Task InterceptAsynchronous_NoAttribute_ProceedsAndReturnsValue()
    {
        var target = new TargetService();
        var services = new ServiceCollection()
            .AddInMemoryCache()
            .AddScoped<TargetService>(_ => target)
            .AddProxiesFromAssembly(typeof(ITargetService).Assembly);
        using var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var proxy = scope.ServiceProvider.GetRequiredService<ITargetService>();

        var value = await proxy.GetNoCacheAsync(2);
        Assert.Equal(2, value);
        Assert.Equal(1, target.CallCount);
    }

    public interface ITargetService : ICacheableService
    {
        Task<int> GetAsync(int id);
        Task<int> GetNoCacheAsync(int id);
    }

    public sealed class TargetService : ITargetService
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
