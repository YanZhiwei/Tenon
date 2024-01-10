using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tenon.Redis.StackExchangeProvider;
using Tenon.Redis.StackExchangeProvider.Extensions;

namespace Tenon.Redis.StackExchangeProviderTests
{
    [TestClass()]
    public class RedisConnectionTests
    {
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
                .AddRedisStackExchangeProvider(configuration.GetSection("Redis"))
                .BuildServiceProvider();
        }

        [TestMethod()]
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

        [TestMethod()]
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
}