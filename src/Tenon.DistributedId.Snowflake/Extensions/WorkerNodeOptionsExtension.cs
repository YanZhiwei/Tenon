using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tenon.DistributedId.Abstractions;
using Tenon.DistributedId.Snowflake.Configurations;
using Tenon.Infra.Redis;
using Tenon.Infra.Redis.Configurations;

namespace Tenon.DistributedId.Snowflake.Extensions;

public sealed class WorkerNodeOptionsExtension<TRedisProvider>(IConfigurationSection workerNodeOptionsSection)
    : IDistributedIdOptionsExtension
    where TRedisProvider : class, IRedisProvider
{
    private readonly IConfigurationSection _workerNodeOptionsSection =
        workerNodeOptionsSection ?? throw new ArgumentNullException(nameof(workerNodeOptionsSection));

    public void AddServices(IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        var workerNodeOptions = _workerNodeOptionsSection.Get<WorkerNodeOptions>();
        if (workerNodeOptions == null)
            throw new ArgumentNullException(nameof(WorkerNodeOptions));
        if (workerNodeOptions.Redis == null)
            throw new ArgumentNullException(nameof(workerNodeOptions.Redis));
        var redisSection = _workerNodeOptionsSection.GetSection(nameof(WorkerNodeOptions.Redis));
        var redisConfig = redisSection.Get<RedisOptions>();
        if (redisConfig == null)
            throw new NullReferenceException(nameof(redisConfig));

        services.AddSingleton<RedisOptions>(redisConfig);
        services.AddSingleton<IRedisProvider, TRedisProvider>();
        services.AddSingleton<WorkerNode>();
    }
}