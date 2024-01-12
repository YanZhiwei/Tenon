using System.Diagnostics;
using Tenon.Helper;
using Timer = System.Timers.Timer;

namespace Tenon.DistributedLocker.Redis;

internal sealed class RedisLockerTimer : Timer
{
    private static readonly string Prefix;

    public readonly string UniqueLockId;

    static RedisLockerTimer()
    {
        using var currentProcess = Process.GetCurrentProcess();
        Prefix = $"{Environment.MachineName}_{currentProcess.Id}";
    }

    public RedisLockerTimer(string cacheKey)
    {
        Checker.Begin().NotNullOrEmpty(cacheKey, nameof(cacheKey));
        UniqueLockId = $"{Prefix}_{cacheKey}";
    }
}