using System.Diagnostics;
using Tenon.Helper;

namespace Tenon.DistributedLocker.Redis;

internal sealed class RedisLockerTimer : IDisposable, IAsyncDisposable
{
    private static readonly string Prefix;

    private readonly Timer _innerTimer;

    public readonly TimeSpan Expiration;

    public readonly string LockKeyId;

    public readonly string LockValue;

    static RedisLockerTimer()
    {
        using var currentProcess = Process.GetCurrentProcess();
        Prefix = $"{Environment.MachineName}_{currentProcess.Id}";
    }

    public RedisLockerTimer(string lockValue, TimerCallback timerCallback, int timeoutSeconds = 5)
    {
        Checker.Begin().NotNullOrEmpty(lockValue, nameof(lockValue))
            .NotNull(timerCallback, nameof(timerCallback));
        LockKeyId = $"{Prefix}_{lockValue}";
        LockValue = lockValue;
        Expiration = TimeSpan.FromSeconds(timeoutSeconds);
        var refreshMilliseconds = (int)(Expiration.TotalMilliseconds / 2.0);
        _innerTimer = new Timer(timerCallback,
            new RedisLockerState(LockKeyId, LockValue, (int)Expiration.TotalMilliseconds), refreshMilliseconds,
            refreshMilliseconds);
        _innerTimer.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _innerTimer.DisposeAsync();
    }


    public void Dispose()
    {
        _innerTimer.Dispose();
    }
}