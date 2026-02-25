using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tenon.Caching.Abstractions;
using Tenon.Caching.RedisStackExchange.Extensions;
using Tenon.Helper.Internal;
using Tenon.Serialization.Json.Extensions;

namespace Tenon.Caching.RedisTests;

[TestClass]
public class RedisStackExchangeCacheProviderTests
{
    private readonly string? _serviceKey;
    private readonly IServiceProvider _serviceProvider;

    public RedisStackExchangeCacheProviderTests()
    {
        _serviceKey = nameof(RedisStackExchangeCacheProviderTests);
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .Build();
        _serviceProvider = new ServiceCollection()
            .AddLogging(loggingBuilder => loggingBuilder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Debug))
            .AddSystemTextJsonSerializer()
            .AddRedisStackExchangeCache(configuration.GetSection("RedisCache2:Redis"))
            .AddKeyedRedisStackExchangeCache(_serviceKey, configuration.GetSection("RedisCache:Redis"))
            .BuildServiceProvider();
    }

    [TestMethod]
    public void KeyedSetTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var cacheKey = $"{RandomHelper.NextString(6, true)}";
            var cacheProvider = scope.ServiceProvider.GetRequiredKeyedService<ICacheProvider>(_serviceKey);
            var actual = cacheProvider.Set(cacheKey, cacheKey, TimeSpan.FromSeconds(5));
            Assert.IsTrue(actual);
            Thread.Sleep(TimeSpan.FromSeconds(10));
            var result = cacheProvider.Exists(cacheKey);
            Assert.IsFalse(result);
        }
    }

    [TestMethod]
    public async Task SetAsyncTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var cacheKey = $"{RandomHelper.NextString(6, true)}";
            var cacheProvider = scope.ServiceProvider.GetService<ICacheProvider>();
            var actual = await cacheProvider.SetAsync(cacheKey, cacheKey, TimeSpan.FromSeconds(5));
            Assert.IsTrue(actual);
            await Task.Delay(TimeSpan.FromSeconds(10));
            var result = await cacheProvider.ExistsAsync(cacheKey);
            Assert.IsFalse(result);
        }
    }
}