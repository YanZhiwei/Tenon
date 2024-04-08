using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Tenon.Caching.Interceptor.Castle;

namespace Tenon.Caching.Interceptor.CastleTests
{
    [TestClass()]
    public class DefaultCacheKeyBuilderTests
    {
        private readonly IServiceProvider _serviceProvider;
        public DefaultCacheKeyBuilderTests()
        {
            Console.WriteLine("DefaultCacheKeyBuilderTests");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();
            _serviceProvider = new ServiceCollection()
                .AddLogging(loggingBuilder => loggingBuilder
                    .SetMinimumLevel(LogLevel.Debug))
                .AddSingleton<ICacheKeyBuilder, DefaultCacheKeyBuilder>()
                .BuildServiceProvider();
        }

        [TestMethod()]
        public void GetCacheKeyTest()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var cacheKeyBuilder = scope.ServiceProvider.GetService<ICacheKeyBuilder>();
                MethodInfo interfaceMethod = typeof(TestService).GetInterfaces()
                    .Where(i => i.GetMethod("CreateAsync") != null)
                    .Select(m => m.GetMethod("CreateAsync")).FirstOrDefault();
                var args = interfaceMethod.GetParameters();
                var actual = cacheKeyBuilder.GetCacheKey(interfaceMethod, args, "test");
                Assert.AreEqual("test:ITestService:CreateAsync:CreationDto:input", actual);
                interfaceMethod = typeof(TestService).GetInterfaces()
                    .Where(i => i.GetMethod("UpdateAsync") != null)
                    .Select(m => m.GetMethod("UpdateAsync")).FirstOrDefault();
                args = interfaceMethod.GetParameters();
                actual = cacheKeyBuilder.GetCacheKey(interfaceMethod, args, "test");
            }
        }

        [TestMethod()]
        public void GetCacheKeysTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetCacheKeyPrefixTest()
        {
            Assert.Fail();
        }
    }
}