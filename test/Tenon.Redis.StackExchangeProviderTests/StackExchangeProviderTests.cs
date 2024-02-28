using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tenon.Infra.Redis;
using Tenon.Infra.Redis.StackExchangeProvider.Extensions;
using Tenon.Serialization.Json.Extensions;

namespace Tenon.Redis.StackExchangeProviderTests;

[TestClass]
public class StackExchangeProviderTests
{
    private readonly string? _serviceKey = nameof(StackExchangeProviderTests);
    private readonly IServiceProvider _serviceProvider;

    public StackExchangeProviderTests()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .Build();
        _serviceProvider = new ServiceCollection()
            .AddLogging(loggingBuilder => loggingBuilder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Debug))
            .AddSystemTextJsonSerializer()
            .AddRedisStackExchangeProvider(configuration.GetSection("Redis"))
            .AddKeyedRedisStackExchangeProvider(_serviceKey,
                configuration.GetSection("Redis2"))
            .BuildServiceProvider();
    }

    [TestMethod]
    public void IncrByTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var redisProvider = scope.ServiceProvider.GetService<IRedisProvider>();
            var actual = redisProvider?.IncrBy($"test_{DateTime.Now:yyyyMMddHHmmss}");
            Assert.AreEqual(1, actual);
        }
    }

    [TestMethod]
    public void KeyedIncrByTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var redisProvider = scope.ServiceProvider.GetRequiredKeyedService<IRedisProvider>(_serviceKey);
            var actual = redisProvider?.IncrBy($"test_{DateTime.Now:yyyyMMddHHmmss}");
            Assert.AreEqual(1, actual);
        }
    }
}