using Tenon.DistributedLocker.Abstractions.Configurations;
using Tenon.Infra.Redis.Configurations;

namespace Tenon.DistributedLocker.Redis.Configurations;

public sealed class RedisDistributedLockerOptions : DistributedLockerOptions
{
    public RedisOptions Redis { get; set; }
}