﻿using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tenon.DistributedLocker.Abstractions;
using Tenon.DistributedLocker.Abstractions.Extensions;
using Tenon.DistributedLocker.RedisStackExchange.Configurations;
using Tenon.Helper.Internal;

namespace Tenon.DistributedLocker.RedisStackExchangeTests;

[TestClass]
public class RedisDistributedLockerTests
{
    private readonly string? _serviceKey = nameof(RedisDistributedLockerTests);
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
            .AddDistributedLocker(options =>
            {
                using var currentProcess = Process.GetCurrentProcess();
                options.KeyPrefix = $"locker_{Environment.MachineName}_{currentProcess.Id}";
                options.UseRedisStackExchange(configuration.GetSection("Redis"));
            })
            .AddDistributedLocker(options =>
            {
                using var currentProcess = Process.GetCurrentProcess();
                options.KeyPrefix = $"locker_{Environment.MachineName}_{currentProcess.Id}";
                options.UseRedisStackExchange(configuration.GetSection("Redis"));
                options.KeyedServiceKey = _serviceKey;
            })
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