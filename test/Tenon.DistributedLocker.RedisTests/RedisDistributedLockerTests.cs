using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tenon.DistributedLocker.Redis.Extensions;
using Tenon.Helper.Internal;
using Tenon.Redis.StackExchangeProvider.Extensions;
using Tenon.Serialization.Json.Extensions;

namespace Tenon.DistributedLocker.RedisTests;

[TestClass]
public class RedisDistributedLockerTests
{
    private readonly string _serviceKey = nameof(RedisDistributedLockerTests);
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
            .AddSystemTextJsonSerializer()
            .AddRedisStackExchangeProvider(configuration.GetSection("DistributedLocker:Redis"))
            .AddKeyedRedisStackExchangeProvider(_serviceKey, configuration.GetSection("DistributedLocker2:Redis"))
            .AddRedisDistributedLocker(configuration.GetSection("DistributedLocker"))
            .AddKeyedRedisDistributedLocker(_serviceKey, configuration.GetSection("DistributedLocker2"))
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
    public async Task KeyedLockAsyncTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var lockKey = $"{RandomHelper.NextString(6, true)}";
            var distributedLocker = scope.ServiceProvider.GetRequiredKeyedService<IDistributedLocker>(_serviceKey);
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

    [TestMethod]
    public async Task KeyedAutoRenewalLockAsyncTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var lockKey = $"{RandomHelper.NextString(6, true)}";
            var distributedLocker = scope.ServiceProvider.GetRequiredKeyedService<IDistributedLocker>(_serviceKey);
            var isLockAcquired = await distributedLocker.LockTakeAsync(lockKey, 10, true);
            Assert.IsTrue(isLockAcquired);
            await Task.Delay(TimeSpan.FromSeconds(60));
            var result = await distributedLocker.LockReleaseAsync(lockKey);
            Assert.IsTrue(result);
        }
    }
}