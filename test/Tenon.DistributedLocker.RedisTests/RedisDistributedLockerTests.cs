using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tenon.DistributedLocker.Redis.Extensions;
using Tenon.Helper.Internal;
using Tenon.Redis.StackExchangeProvider;
using Tenon.Redis.StackExchangeProvider.Extensions;
using Tenon.Serialization.Json;

namespace Tenon.DistributedLocker.RedisTests;

[TestClass]
public class RedisDistributedLockerTests
{
    private readonly IServiceProvider _serviceProvider;

    public RedisDistributedLockerTests()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .Build();
        _serviceProvider = new ServiceCollection()
            .AddLogging(loggingBuilder => loggingBuilder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Debug))
            .AddRedisStackExchangeProvider<JsonSerializer>(configuration.GetSection("Redis"))
            .AddRedisDistributedLocker<StackExchangeProvider>(configuration.GetSection("DistributedLocker"))
            .BuildServiceProvider();
    }

    [TestMethod]
    public async Task LockAsyncTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var lockKey = $"{RandomHelper.NextString(6, true)}";
            var distributedLocker = scope.ServiceProvider.GetService<IDistributedLocker>();
            var isLockAcquired = await distributedLocker.LockTakeAsync(lockKey, 60);
            Assert.IsTrue(isLockAcquired);
            var result = await distributedLocker.LockReleaseAsync(lockKey);
            Assert.IsTrue(result);
        }
    }

    [TestMethod]
    public async Task AutoRenewalLockAsyncTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var lockKey = $"{RandomHelper.NextString(6, true)}";
            var distributedLocker = scope.ServiceProvider.GetService<IDistributedLocker>();
            var isLockAcquired = await distributedLocker.LockTakeAsync(lockKey, 10, true);
            Assert.IsTrue(isLockAcquired);
            await Task.Delay(TimeSpan.FromSeconds(60));
            var result = await distributedLocker.LockReleaseAsync(lockKey);
            Assert.IsTrue(result);
        }
    }
}