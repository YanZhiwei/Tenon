using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tenon.Caching.Abstractions;
using Tenon.DistributedId.Abstractions;
using Tenon.DistributedId.Abstractions.Extensions;
using Tenon.DistributedId.Snowflake;
using Tenon.DistributedId.Snowflake.Configurations;
using Tenon.Infra.Redis;
using Tenon.Infra.Redis.StackExchangeProvider;

namespace SnowflakeSample;

internal class Program
{
    private static IServiceProvider _serviceProvider;

    private static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        try
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();
            _serviceProvider = new ServiceCollection()
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.AddConsole();
                    loggingBuilder.SetMinimumLevel(LogLevel.Debug);
                    loggingBuilder.AddSimpleConsole(options =>
                    {
                        options.IncludeScopes = true;
                        options.SingleLine = true;
                        options.TimestampFormat = "HH:mm:ss ";
                    });
                })
                .AddDistributedId(options =>
                {
                    options.UseSnowflake(configuration.GetSection("DistributedId"));
                    options.UseWorkerNode<StackExchangeProvider>(configuration.GetSection("DistributedId")
                        .GetSection("WorkerNode"));
                })
                .BuildServiceProvider();
            SnowflakeIdGenerator idGenerator = new SnowflakeIdGenerator();
            idGenerator.SetWorkerId(63);
            Console.WriteLine(idGenerator.GetNextId());
            using (var scope = _serviceProvider.CreateScope())
            {
                var iDGenerator = scope.ServiceProvider.GetService<IDGenerator>();
                var redisProvider = scope.ServiceProvider.GetService<IRedisProvider>();
                var workerNode = scope.ServiceProvider.GetService<WorkerNode>();
                await workerNode?.RegisterAsync()!;
                Console.WriteLine(iDGenerator.GetNextId());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}