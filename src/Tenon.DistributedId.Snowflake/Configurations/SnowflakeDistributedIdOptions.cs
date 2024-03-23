using Microsoft.Extensions.Configuration;
using Tenon.DistributedId.Abstractions.Configurations;
using Tenon.DistributedId.Snowflake.Extensions;
using Tenon.Infra.Redis;

namespace Tenon.DistributedId.Snowflake.Configurations;

public static class SnowflakeDistributedIdOptions
{
    public static DistributedIdOptions UseSnowflake(this DistributedIdOptions options,
        IConfigurationSection snowflakeIdOptionsSection)
    {
        options.RegisterExtension(new SnowflakeOptionsExtension(snowflakeIdOptionsSection));
        return options;
    }

    public static DistributedIdOptions UseWorkerNode<TRedisProvider>(this DistributedIdOptions options,
        IConfigurationSection workerNodeOptionsSection) where TRedisProvider : class, IRedisProvider
    {
        options.RegisterExtension(new WorkerNodeOptionsExtension<TRedisProvider>(workerNodeOptionsSection));
        return options;
    }
}