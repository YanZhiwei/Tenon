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
                .AddLogging(loggingBuilder => loggingBuilder
                    .SetMinimumLevel(LogLevel.Debug))
                .AddDistributedId(options =>
                {
                    options.UseSnowflake(configuration.GetSection("DistributedId"));
                    options.UseWorkerNode<StackExchangeProvider>(configuration.GetSection("DistributedId")
                        .GetSection("WorkerNode"));
                })
                .BuildServiceProvider();

            using (var scope = _serviceProvider.CreateScope())
            {
                var iDGenerator = scope.ServiceProvider.GetService<IDGenerator>();
                var redisProvider = scope.ServiceProvider.GetService<IRedisProvider>();
                var workerNode = scope.ServiceProvider.GetService<WorkerNode>();
                await workerNode?.RegisterAsync()!;
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