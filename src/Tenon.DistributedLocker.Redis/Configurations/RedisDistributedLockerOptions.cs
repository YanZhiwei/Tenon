using Tenon.DistributedLocker.Configurations;
using Tenon.Redis.Configurations;

namespace Tenon.DistributedLocker.Redis.Configurations;

public sealed class RedisDistributedLockerOptions : DistributedLockerOptions
{
    public RedisOptions Redis { get; set; }
}