using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tenon.Redis.StackExchangeProvider.Extensions;

namespace Tenon.Redis.StackExchangeProviderTests;

[TestClass]
public class StackExchangeProviderTests
{
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
            .AddRedisStackExchangeProvider<Tenon.Serialization.Json.SystemTextJsonSerializer>(configuration.GetSection("Redis"))
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
}