namespace Tenon.DistributedLocker.Configurations;

public sealed class DistributedLockerOptions
{
    public string LockKeyPrefix { get; set; } = string.Empty;
}