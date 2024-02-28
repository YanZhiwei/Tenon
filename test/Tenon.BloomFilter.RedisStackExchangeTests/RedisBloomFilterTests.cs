using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tenon.BloomFilter.Abstractions;
using Tenon.BloomFilter.Abstractions.Configurations;
using Tenon.BloomFilter.Abstractions.Extensions;
using Tenon.BloomFilter.RedisStackExchange.Configurations;

namespace Tenon.BloomFilter.RedisStackExchangeTests;

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
            .AddBloomFilter(opt => { opt.UseRedisStackExchange(configuration.GetSection("Redis")); })
            .AddBloomFilter(opt =>
            {
                opt.KeyedServices = true;
                opt.KeyedServiceKey = "RedisBloomFilterTests";
                opt.UseRedisStackExchange(configuration.GetSection("Redis"));
            })
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
    public async Task KeyedAddAsyncTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var bloomFilterOptions = _serviceProvider.GetRequiredService<IOptions<BloomFilterOptions>>().Value;
            var serviceKey = bloomFilterOptions.KeyedServiceKey;
            var bloomFilter = scope.ServiceProvider.GetKeyedService<IBloomFilter>(serviceKey);
            var actual = await bloomFilter.AddAsync(_boomFilterKey, "zhangsan");
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
    public async Task KeyedAddRangeAsyncTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var bloomFilterOptions = _serviceProvider.GetRequiredService<IOptions<BloomFilterOptions>>().Value;
            var serviceKey = bloomFilterOptions.KeyedServiceKey;
            var bloomFilter = scope.ServiceProvider.GetKeyedService<IBloomFilter>(serviceKey);
            var actual = await bloomFilter.AddAsync(_boomFilterKey, new[] { "zhangsan1", "zhangsan2", "zhangsan3" });
            Assert.IsNotNull(actual);
            Assert.AreEqual(3, actual.Length);
            Assert.IsTrue(actual[0]);
            Assert.IsTrue(actual[1]);
            Assert.IsTrue(actual[2]);
        }
    }

    [TestMethod]
    public async Task KeyedExistsAsyncTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var bloomFilterOptions = _serviceProvider.GetRequiredService<IOptions<BloomFilterOptions>>().Value;
            var serviceKey = bloomFilterOptions.KeyedServiceKey;
            var bloomFilter = scope.ServiceProvider.GetKeyedService<IBloomFilter>(serviceKey);
            Debug.WriteLine($"ExistsAsyncTest key:{_boomFilterKey}");
            var actual = await bloomFilter.ExistsAsync(_boomFilterKey, _boomFilterDefaultValue);
            Assert.IsTrue(actual);
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


    [TestMethod]
    public async Task KeyedExistsRangeAsyncTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var bloomFilterOptions = _serviceProvider.GetRequiredService<IOptions<BloomFilterOptions>>().Value;
            var serviceKey = bloomFilterOptions.KeyedServiceKey;
            var bloomFilter = scope.ServiceProvider.GetKeyedService<IBloomFilter>(serviceKey);
            var actual = await bloomFilter.ExistsAsync(_boomFilterKey, new[] { _boomFilterDefaultValue });
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Length);
            Assert.IsTrue(actual[0]);
        }
    }
}