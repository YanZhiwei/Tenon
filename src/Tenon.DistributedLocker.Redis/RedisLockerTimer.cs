using System.Diagnostics;
using Tenon.Helper;
using Timer = System.Timers.Timer;

namespace Tenon.DistributedLocker.Redis;

internal sealed class RedisLockerTimer
{
    private static readonly string Prefix;

    public readonly string UniqueLockId;

    private readonly Timer _innerTimer;

    static RedisLockerTimer()
    {
        using var currentProcess = Process.GetCurrentProcess();
        Prefix = $"{Environment.MachineName}_{currentProcess.Id}";
    }

    public RedisLockerTimer(string cacheKey, TimerCallback timerCallback, int timeoutSeconds = 5)
    {
        Checker.Begin().NotNullOrEmpty(cacheKey, nameof(cacheKey));
        UniqueLockId = $"{Prefix}_{cacheKey}";
        var expiration = TimeSpan.FromSeconds(timeoutSeconds);
        var refreshMilliseconds = (int)(expiration.TotalMilliseconds / 2.0);
        _innerTimer = new System.Timers.Timer(() => { }, null, refreshMilliseconds, refreshMilliseconds);
    }
}