using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tenon.Caching.Abstractions;
using Tenon.Caching.Abstractions.Configurations;
using Tenon.Caching.Abstractions.Extensions;
using Tenon.Caching.RedisStackExchange.Configurations;
using Tenon.Helper.Internal;

namespace Tenon.Caching.RedisStackExchangeTests;

[TestClass]
public class RedisCacheProviderTests
{
    private static readonly string _cacheKey = $"{RandomHelper.NextString(6, true)}";
    private static readonly string _keyedCacheKey = $"{RandomHelper.NextString(6, true)}";
    private readonly IServiceProvider _serviceProvider;

    static RedisCacheProviderTests()
    {
    }

    public RedisCacheProviderTests()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .Build();
        _serviceProvider = new ServiceCollection()
            .AddLogging(loggingBuilder => loggingBuilder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Debug))
            .AddCaching(opt =>
            {
                opt.KeyedServiceKey = $"{nameof(CachingOptions).ToLower()}:test";
                opt.KeyedServices = true;
                opt.MaxRandomSecond = 10;
                opt.Prefix = "RedisCacheProviderTests".ToLower();
                opt.UseSystemTextJsonSerializer();
                opt.UseRedisStackExchange(configuration.GetSection("Redis"));
            })
            .AddCaching(opt =>
            {
                opt.MaxRandomSecond = 10;
                opt.Prefix = "RedisCacheProviderTests".ToLower();
                opt.UseSystemTextJsonSerializer();
                opt.UseRedisStackExchange(configuration.GetSection("Redis"));
            })
            .BuildServiceProvider();
    }

    [TestMethod]
    public void A_KeyedSetTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var cachingOptions =
                _serviceProvider.GetKeyedService<CachingOptions>($"{nameof(CachingOptions).ToLower()}:test");
            var serviceKey = cachingOptions.KeyedServiceKey;
            var cacheProvider =
                scope.ServiceProvider.GetRequiredKeyedService<ICacheProvider>(serviceKey);
            Console.WriteLine($"{nameof(A_KeyedSetTest)} key:{_keyedCacheKey}");
            var actual = cacheProvider.Set(_keyedCacheKey, RandomHelper.NextString(6, true), TimeSpan.FromSeconds(160));
            Assert.IsTrue(actual);
        }
    }

    [TestMethod]
    public async Task C_KeyedRemoveAllAsyncTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var cachingOptions =
                _serviceProvider.GetKeyedService<CachingOptions>($"{nameof(CachingOptions).ToLower()}:test");
            var serviceKey = cachingOptions.KeyedServiceKey;
            var cacheProvider =
                scope.ServiceProvider.GetRequiredKeyedService<ICacheProvider>(serviceKey);
            Console.WriteLine($"{nameof(C_KeyedRemoveAllAsyncTest)} keys:{_cacheKey},{_keyedCacheKey}");
            var actual = await cacheProvider.RemoveAllAsync(new[] { _cacheKey, _keyedCacheKey });
            Assert.IsTrue(actual == 2);
        }
    }

    [TestMethod]
    public async Task B_SetAsyncTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var cacheProvider = scope.ServiceProvider.GetService<ICacheProvider>();
            Console.WriteLine($"{nameof(B_SetAsyncTest)} key:{_cacheKey}");
            var actual =
                await cacheProvider?.SetAsync(_cacheKey, RandomHelper.NextString(6, true), TimeSpan.FromSeconds(160))!;
            Assert.IsTrue(actual);
        }
    }
}