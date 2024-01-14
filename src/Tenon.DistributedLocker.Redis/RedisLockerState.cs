namespace Tenon.DistributedLocker.Redis;

internal sealed class RedisLockerState(string lockKey, string lockValue, int milliSeconds)
{
    public readonly string LockKey = lockKey;
    public readonly double MilliSeconds = milliSeconds;
    public readonly string LockValue = lockValue;
}