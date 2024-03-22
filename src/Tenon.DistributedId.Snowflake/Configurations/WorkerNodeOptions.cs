namespace Tenon.DistributedId.Snowflake.Configurations;

public sealed class WorkerNodeOptions
{
    public string Prefix { get; set; } = "distributedId:workerIds:";

    public int ExpireTimeInSeconds { get; set; } = 60;

    public int RefreshTimeInSeconds { get; set; }
}