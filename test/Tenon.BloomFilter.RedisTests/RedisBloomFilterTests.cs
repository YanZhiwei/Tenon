﻿using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tenon.BloomFilter.Abstractions;
using Tenon.BloomFilter.Redis.Configurations;
using Tenon.Infra.Redis.StackExchangeProvider.Extensions;
using Tenon.Serialization.Json.Extensions;

namespace Tenon.BloomFilter.RedisTests;

[TestClass]
public class RedisBloomFilterTests
{
    private readonly string _boomFilterDefaultValue;
    private readonly string _boomFilterKey;
    private readonly IServiceProvider _serviceProvider;

    public RedisBloomFilterTests()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .Build();
        _serviceProvider = new ServiceCollection()
            .AddLogging(loggingBuilder => loggingBuilder
                .SetMinimumLevel(LogLevel.Debug))
            .AddSystemTextJsonSerializer()
            .AddRedisStackExchangeProvider(configuration.GetSection("Redis"))
            .AddRedisBloomFilter()
            .BuildServiceProvider();
        _boomFilterKey = $"Tenon.BloomFilter.RedisTests:{DateTime.Now.ToString("yyyyMMddHHmmss")}";
        _boomFilterDefaultValue = $"zhangsan_{DateTime.Now.ToString("yyyyMMddHHmmss")}";
    }

    [TestInitialize]
    public async Task Init()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var bloomFilter = scope.ServiceProvider.GetService<IBloomFilter>();
            await bloomFilter.ReserveAsync(_boomFilterKey, 0.01, 1000);
            var actual = await bloomFilter.AddAsync(_boomFilterKey, _boomFilterDefaultValue);
            Assert.IsTrue(actual);
        }
    }

    [TestMethod]
    public async Task AddAsyncTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var bloomFilter = scope.ServiceProvider.GetService<IBloomFilter>();
            var actual = await bloomFilter.AddAsync(_boomFilterKey, "zhangsan");
            Assert.IsTrue(actual);
        }
    }

    [TestMethod]
    public async Task AddRangeAsyncTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var bloomFilter = scope.ServiceProvider.GetService<IBloomFilter>();
            var actual = await bloomFilter.AddAsync(_boomFilterKey, new[] { "zhangsan1", "zhangsan2", "zhangsan3" });
            Assert.IsNotNull(actual);
            Assert.AreEqual(3, actual.Length);
            Assert.IsTrue(actual[0]);
            Assert.IsTrue(actual[1]);
            Assert.IsTrue(actual[2]);
        }
    }

    [TestMethod]
    public async Task ExistsAsyncTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var bloomFilter = scope.ServiceProvider.GetService<IBloomFilter>();
            Debug.WriteLine($"ExistsAsyncTest key:{_boomFilterKey}");
            var actual = await bloomFilter.ExistsAsync(_boomFilterKey, _boomFilterDefaultValue);
            Assert.IsTrue(actual);
        }
    }

    [TestMethod]
    public async Task ExistsRangeAsyncTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var bloomFilter = scope.ServiceProvider.GetService<IBloomFilter>();
            var actual = await bloomFilter.ExistsAsync(_boomFilterKey, new[] { _boomFilterDefaultValue });
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Length);
            Assert.IsTrue(actual[0]);
        }
    }
}