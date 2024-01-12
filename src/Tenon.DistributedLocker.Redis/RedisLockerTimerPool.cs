using System.Collections.Concurrent;

namespace Tenon.DistributedLocker.Redis;

internal sealed class RedisLockerTimerPool
{
    private static readonly ConcurrentDictionary<string, RedisLockerTimer> Timers = new();

    public RedisLockerTimerPool()
    {

    }

    public static bool TryAdd(RedisLockerTimer redisLocker)
    {
        return Timers.TryAdd(redisLocker.UniqueLockId, redisLocker);
    }

    public void Close(string key)
    {
        if (Timers.ContainsKey(key))
        {
            if (Timers.TryRemove(key, out var timer))
            {
                timer?.Stop();
                timer?.Dispose();
            }
        }
    }

    public bool Contains(string key)
    {
        return Timers.ContainsKey(key);
    }
}