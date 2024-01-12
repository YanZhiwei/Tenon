namespace Tenon.DistributedLocker.Redis;

internal sealed class RedisLockerState(string lockKeyId, string lockValue, int milliseconds)
{
    public readonly string LockKeyId = lockKeyId;
    public readonly int Milliseconds = milliseconds;
    public readonly string LockValue = lockValue;
}