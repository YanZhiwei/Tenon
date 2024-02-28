using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
    private readonly IServiceProvider _serviceProvider;

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
                opt.UseSystemTextJsonSerializer();
                opt.UseRedisStackExchange(configuration.GetSection("RedisCache"));
            })
            .AddCaching(opt =>
            {
                opt.UseSystemTextJsonSerializer();
                opt.UseRedisStackExchange(configuration.GetSection("RedisCache2"));
            })
            //.AddKeyedRedisStackExchangeCache($"{nameof(CachingOptions).ToLower()}:test", configuration.GetSection("RedisCache"))
            .BuildServiceProvider();
    }

    [TestMethod]
    public void KeyedSetTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var cachingOptions = _serviceProvider.GetRequiredService<IOptions<CachingOptions>>().Value;
            var serviceKey = cachingOptions.KeyedServiceKey; //$"{nameof(CachingOptions).ToLower()}:test";//
            var cacheKey = $"{RandomHelper.NextString(6, true)}";
            var cacheProvider =
                scope.ServiceProvider.GetRequiredKeyedService<ICacheProvider>(serviceKey);
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
            var actual = await cacheProvider?.SetAsync(cacheKey, cacheKey, TimeSpan.FromSeconds(5))!;
            Assert.IsTrue(actual);
            await Task.Delay(TimeSpan.FromSeconds(10));
            var result = await cacheProvider.ExistsAsync(cacheKey);
            Assert.IsFalse(result);
        }
    }
}