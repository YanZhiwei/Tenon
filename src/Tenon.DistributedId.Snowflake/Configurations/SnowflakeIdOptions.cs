namespace Tenon.DistributedId.Snowflake.Configurations;

public class SnowflakeIdOptions
{
    public string ServiceName { get; set; }

    public WorkerNodeOptions WorkerNode { get; set; }
}