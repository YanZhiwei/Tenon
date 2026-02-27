using Microsoft.Extensions.DependencyInjection;
using Tenon.Caching.Abstractions;
using Tenon.Caching.Abstractions.Extensions;
using Tenon.Caching.InMemory.Configurations;
using Tenon.Caching.InMemory.Extensions;
using Xunit;

namespace Tenon.Caching.InMemoryTests;

/// <summary>
/// 验证 AddInMemoryCache 与 AddCaching + UseInMemoryStorage 的 DI 注册及 Set/Get 行为。
/// </summary>
public sealed class ServiceCollectionExtensionTests
{
    [Fact]
    public void AddInMemoryCache_ResolvesICacheProvider_AndSetGetSucceeds()
    {
        var services = new ServiceCollection()
            .AddInMemoryCache();

        using var provider = services.BuildServiceProvider();
        var cache = provider.GetRequiredService<ICacheProvider>();

        Assert.NotNull(cache);

        const string key = "test:key1";
        const string value = "value1";
        Assert.True(cache.Set(key, value, TimeSpan.FromMinutes(1)));

        var result = cache.Get<string>(key);
        Assert.True(result.HasValue);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public void AddCaching_UseInMemoryStorage_ResolvesICacheProvider_AndSetGetSucceeds()
    {
        var services = new ServiceCollection()
            .AddCaching(options => options.UseInMemoryStorage());

        using var provider = services.BuildServiceProvider();
        var cache = provider.GetRequiredService<ICacheProvider>();

        Assert.NotNull(cache);

        const string key = "test:key2";
        const string value = "value2";
        Assert.True(cache.Set(key, value, TimeSpan.FromMinutes(1)));

        var result = cache.Get<string>(key);
        Assert.True(result.HasValue);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public void AddInMemoryCache_Remove_AndExists_BehaveCorrectly()
    {
        var services = new ServiceCollection()
            .AddInMemoryCache();

        using var provider = services.BuildServiceProvider();
        var cache = provider.GetRequiredService<ICacheProvider>();

        const string key = "test:remove";
        cache.Set(key, "x", TimeSpan.FromMinutes(1));
        Assert.True(cache.Exists(key));

        Assert.True(cache.Remove(key));
        Assert.False(cache.Exists(key));
        Assert.False(cache.Get<string>(key).HasValue);
    }

    [Fact]
    public void AddInMemoryCache_RemoveAll_ReturnsCorrectCount()
    {
        var services = new ServiceCollection()
            .AddInMemoryCache();

        using var provider = services.BuildServiceProvider();
        var cache = provider.GetRequiredService<ICacheProvider>();

        cache.Set("a", 1, TimeSpan.FromMinutes(1));
        cache.Set("b", 2, TimeSpan.FromMinutes(1));
        cache.Set("c", 3, TimeSpan.FromMinutes(1));

        var removed = cache.RemoveAll(new[] { "a", "b", "c" });
        Assert.Equal(3, removed);
        Assert.False(cache.Exists("a"));
        Assert.False(cache.Exists("b"));
        Assert.False(cache.Exists("c"));
    }

    [Fact]
    public void AddInMemoryCache_WithConfigureOptions_ResolvesICacheProvider_AndSetGetSucceeds()
    {
        var services = new ServiceCollection()
            .AddInMemoryCache(options =>
            {
                options.CacheName = "TestNamedCache";
                options.CacheMemoryLimitMegabytes = 10;
                options.PollingInterval = TimeSpan.FromMinutes(1);
            });

        using var provider = services.BuildServiceProvider();
        var cache = provider.GetRequiredService<ICacheProvider>();

        Assert.NotNull(cache);

        const string key = "test:configured";
        const string value = "configured_value";
        Assert.True(cache.Set(key, value, TimeSpan.FromMinutes(1)));

        var result = cache.Get<string>(key);
        Assert.True(result.HasValue);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public void AddInMemoryCache_WithNullConfigureOptions_BehavesLikeParameterless()
    {
        var services = new ServiceCollection()
            .AddInMemoryCache((Action<InMemoryCacheOptions>?)null);

        using var provider = services.BuildServiceProvider();
        var cache = provider.GetRequiredService<ICacheProvider>();

        Assert.NotNull(cache);
        Assert.True(cache.Set("k", "v", TimeSpan.FromMinutes(1)));
        Assert.True(cache.Get<string>("k").HasValue);
    }
}
