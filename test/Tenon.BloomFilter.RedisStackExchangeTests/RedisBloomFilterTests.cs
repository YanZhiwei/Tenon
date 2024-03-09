using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tenon.BloomFilter.Abstractions;
using Tenon.BloomFilter.Abstractions.Configurations;
using Tenon.BloomFilter.Abstractions.Extensions;
using Tenon.BloomFilter.RedisStackExchange.Configurations;
using Tenon.Helper.Internal;

namespace Tenon.BloomFilter.RedisStackExchangeTests;

[TestClass]
public class RedisBloomFilterTests
{
    private static readonly string _keyedBoomFilterValue;
    private static readonly string[] _keyedBoomFilterValues;
    private static readonly string _boomFilterValue;
    private static readonly string[] _boomFilterValues;
    private readonly IServiceProvider _serviceProvider;

    static RedisBloomFilterTests()
    {
        _boomFilterValue = $"zhangsan_{RandomHelper.NextHexString(10)}";
        _boomFilterValues = new[]
        {
            $"zhangsan_{RandomHelper.NextHexString(10)}",
            $"zhangsan_{RandomHelper.NextHexString(10)}",
            $"zhangsan_{RandomHelper.NextHexString(10)}"
        };
        _keyedBoomFilterValue = $"zhangsan_{RandomHelper.NextHexString(20)}";
        _keyedBoomFilterValues = new[]
        {
            $"zhangsan_{RandomHelper.NextHexString(20)}",
            $"zhangsan_{RandomHelper.NextHexString(20)}",
            $"zhangsan_{RandomHelper.NextHexString(20)}"
        };
    }

    public RedisBloomFilterTests()
    {
        Console.WriteLine("RedisBloomFilterTests");
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .Build();
        _serviceProvider = new ServiceCollection()
            .AddLogging(loggingBuilder => loggingBuilder
                .SetMinimumLevel(LogLevel.Debug))
            .AddBloomFilter(opt =>
            {
                opt.Name = "test";
                opt.Capacity = 1000;
                opt.ErrorRate = 0.01;
                opt.UseRedisStackExchange(configuration.GetSection("Redis"));
            })
            .AddBloomFilter(opt =>
            {
                opt.Name = "test1";
                opt.Capacity = 1000;
                opt.ErrorRate = 0.01;
                opt.UseRedisStackExchange(configuration.GetSection("Redis"));
            })
            .AddBloomFilter(opt =>
            {
                opt.Name = "test2";
                opt.Capacity = 1000;
                opt.ErrorRate = 0.01;
                opt.UseRedisStackExchange(configuration.GetSection("Redis"));
            })
            .AddBloomFilter(opt =>
            {
                opt.Name = "testKeyed";
                opt.Capacity = 1000;
                opt.ErrorRate = 0.01;
                opt.KeyedServiceKey = "RedisBloomFilterTests";
                opt.UseRedisStackExchange(configuration.GetSection("Redis"));
            })
            .AddBloomFilter(opt =>
            {
                opt.Name = "testKeyed1";
                opt.Capacity = 1000;
                opt.ErrorRate = 0.01;
                opt.KeyedServiceKey = "RedisBloomFilterTests1";
                opt.UseRedisStackExchange(configuration.GetSection("Redis"));
            })
            .BuildServiceProvider();
    }

    [TestInitialize]
    public async Task Init()
    {
        Console.WriteLine("init");
        using (var scope = _serviceProvider.CreateScope())
        {
            var bloomFilter = scope.ServiceProvider.GetService<IBloomFilter>();
            await bloomFilter.InitAsync();
            Assert.IsTrue(await bloomFilter.ExistsAsync());
        }

        using (var scope = _serviceProvider.CreateScope())
        {
            var bloomFilterOptions =
                scope.ServiceProvider.GetKeyedService<BloomFilterOptions>("RedisBloomFilterTests1");
            var serviceKey = bloomFilterOptions.KeyedServiceKey;
            var bloomFilter = scope.ServiceProvider.GetKeyedService<IBloomFilter>(serviceKey);
            await bloomFilter.InitAsync();
            Assert.IsTrue(await bloomFilter.ExistsAsync());
        }
    }


    [TestMethod]
    public async Task KeyedAddAsyncTest_5()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var bloomFilterOptions =
                scope.ServiceProvider.GetKeyedService<BloomFilterOptions>("RedisBloomFilterTests1");
            var serviceKey = bloomFilterOptions.KeyedServiceKey;
            var bloomFilter = scope.ServiceProvider.GetKeyedService<IBloomFilter>(serviceKey);
            var actual = await bloomFilter.AddAsync(_keyedBoomFilterValue);
            Assert.IsTrue(actual);
        }
    }

    [TestMethod]
    public async Task AddAsyncTest_1()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var bloomFilter = scope.ServiceProvider.GetService<IBloomFilter>();
            var bloomFilters = scope.ServiceProvider.GetServices<IBloomFilter>();
            Assert.AreEqual(3, bloomFilters?.Count());

            Console.WriteLine($"AddAsyncTest_1 value {_boomFilterValue}");
            var actual = await bloomFilter.AddAsync(_boomFilterValue);
            Assert.IsTrue(actual);
        }
    }

    [TestMethod]
    public async Task AddRangeAsyncTest_2()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var bloomFilter = scope.ServiceProvider.GetService<IBloomFilter>();
            Console.WriteLine($"AddRangeAsyncTest_2 values {string.Join(",", _boomFilterValues)}");
            var actual = await bloomFilter.AddAsync(_boomFilterValues);
            Assert.IsNotNull(actual);
            Assert.AreEqual(3, actual.Length);
            Assert.IsTrue(actual[0]);
            Assert.IsTrue(actual[1]);
            Assert.IsTrue(actual[2]);
        }
    }


    [TestMethod]
    public async Task KeyedAddRangeAsyncTest_6()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var bloomFilterOptions =
                scope.ServiceProvider.GetKeyedService<BloomFilterOptions>("RedisBloomFilterTests1");
            var serviceKey = bloomFilterOptions.KeyedServiceKey;
            var bloomFilter = scope.ServiceProvider.GetKeyedService<IBloomFilter>(serviceKey);
            var actual = await bloomFilter.AddAsync(_keyedBoomFilterValues);
            Assert.IsNotNull(actual);
            Assert.AreEqual(_keyedBoomFilterValues.Length, actual.Length);
            Assert.IsTrue(actual[0]);
            Assert.IsTrue(actual[1]);
            Assert.IsTrue(actual[2]);
        }
    }

    [TestMethod]
    public async Task KeyedExistsAsyncTest_7()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var bloomFilterOptions =
                scope.ServiceProvider.GetKeyedService<BloomFilterOptions>("RedisBloomFilterTests1");
            var serviceKey = bloomFilterOptions.KeyedServiceKey;
            var bloomFilter = scope.ServiceProvider.GetKeyedService<IBloomFilter>(serviceKey);
            var actual = await bloomFilter.ExistsAsync(_keyedBoomFilterValue);
            Assert.IsTrue(actual);
        }
    }

    [TestMethod]
    public async Task ExistsAsyncTest_3()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            Console.WriteLine($"ExistsAsyncTest_3 value {_boomFilterValue}");
            var bloomFilter = scope.ServiceProvider.GetService<IBloomFilter>();
            var actual = await bloomFilter.ExistsAsync(_boomFilterValue);
            Assert.IsTrue(actual);
        }
    }

    [TestMethod]
    public async Task ExistsRangeAsyncTest_4()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var bloomFilter = scope.ServiceProvider.GetService<IBloomFilter>();
            Console.WriteLine($"{nameof(ExistsRangeAsyncTest_4)} values {string.Join(",", _boomFilterValues)}");
            var actual = await bloomFilter.ExistsAsync(_boomFilterValues);
            Assert.IsNotNull(actual);
            Assert.AreEqual(_boomFilterValues.Length, actual.Length);
            Assert.IsTrue(actual[0]);
        }
    }


    [TestMethod]
    public async Task KeyedExistsRangeAsyncTest_8()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var bloomFilterOptions =
                scope.ServiceProvider.GetKeyedService<BloomFilterOptions>("RedisBloomFilterTests1");
            var serviceKey = bloomFilterOptions.KeyedServiceKey;
            Console.WriteLine(
                $"{nameof(KeyedExistsRangeAsyncTest_8)} values {string.Join(",", _keyedBoomFilterValues)}");
            var bloomFilter = scope.ServiceProvider.GetKeyedService<IBloomFilter>(serviceKey);
            var actual = await bloomFilter.ExistsAsync(_keyedBoomFilterValues);
            Assert.IsNotNull(actual);
            Assert.AreEqual(_boomFilterValues.Length, actual.Length);
            Assert.IsTrue(actual[0]);
        }
    }
}