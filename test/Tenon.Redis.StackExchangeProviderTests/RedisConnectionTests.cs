using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tenon.Redis.StackExchangeProvider;
using Tenon.Redis.StackExchangeProvider.Extensions;
using Tenon.Serialization.Json.Extensions;

namespace Tenon.Redis.StackExchangeProviderTests;

[TestClass]
public class RedisConnectionTests
{
    private readonly string? _serviceKey = nameof(RedisConnectionTests);
    private readonly IServiceProvider _serviceProvider;

    public RedisConnectionTests()
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
            .AddKeyedRedisStackExchangeProvider(_serviceKey, configuration.GetSection("Redis2"))
            .AddKeyedRedisStackExchangeProvider("abc", configuration.GetSection("Redis2"))
            .BuildServiceProvider();
    }

    [TestMethod]
    public void GetDatabaseTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var redisDataBase = scope.ServiceProvider.GetService<RedisConnection>();
            Assert.IsNotNull(redisDataBase);
            var database = redisDataBase.GetDatabase();
            Assert.AreEqual(0, database.Database);
        }
    }

    [TestMethod]
    public void GetKeyedDatabaseTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var redisDataBase = scope.ServiceProvider.GetKeyedService<RedisConnection>(_serviceKey);
            Assert.IsNotNull(redisDataBase);
            var database = redisDataBase.GetDatabase();
            Assert.AreEqual(2, database.Database);
        }
    }

    [TestMethod]
    public void GetStandaloneServersTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var redisDataBase = scope.ServiceProvider.GetService<RedisConnection>();
            Assert.IsNotNull(redisDataBase);
            var redisServer = redisDataBase.GetServers();
            Assert.IsTrue(redisServer.Any());
            var standaloneServer = redisServer.FirstOrDefault();
            Assert.IsNotNull(standaloneServer);
        }
    }
}