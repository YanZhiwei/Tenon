﻿using System.Collections.Concurrent;
using System.Diagnostics;
using Tenon.DistributedLocker.Abstractions;
using Tenon.DistributedLocker.Abstractions.Configurations;
using Tenon.Infra.Redis;

namespace Tenon.DistributedLocker.Redis;

public sealed class RedisDistributedLocker : IDistributedLocker
{
    private static readonly ConcurrentDictionary<string, Timer> AutoRenewalTimers;
    private readonly DistributedLockerOptions? _distributedLockerOptions;
    private readonly string _prefix;
    private readonly IRedisProvider _redisProvider;

    static RedisDistributedLocker()
    {
        AutoRenewalTimers = new ConcurrentDictionary<string, Timer>();
        using var currentProcess = Process.GetCurrentProcess();
    }


    public RedisDistributedLocker(IRedisProvider redisProvider,
        DistributedLockerOptions distributedLockerOptions)
    {
        _redisProvider = redisProvider ?? throw new ArgumentNullException(nameof(redisProvider));
        _distributedLockerOptions = distributedLockerOptions ??
                                    throw new ArgumentNullException(nameof(distributedLockerOptions));
        _prefix = distributedLockerOptions.KeyPrefix;
    }

    public async Task<bool> LockTakeAsync(string cacheKey, int timeoutSeconds = 30, bool autoDelay = false)
    {
        var lockKey = CreateLockKey(cacheKey);
        var lockValue = cacheKey;
        var expiration = TimeSpan.FromSeconds(timeoutSeconds);
        var flag = await _redisProvider.StringSetAsync(lockKey, lockValue, expiration, StringSetWhen.NotExists);
        if (!autoDelay) return flag;
        if (!flag) return flag;
        var refreshMilliseconds = (int)(expiration.TotalMilliseconds / 2.0);
        var renewalTimer = new Timer(RenewalTimerWorker,
            new RedisLockerState(lockKey, lockValue, (int)expiration.TotalMilliseconds), refreshMilliseconds,
            refreshMilliseconds);
        if (!AutoRenewalTimers.TryAdd(lockKey, renewalTimer))
        {
            await renewalTimer.DisposeAsync();
            return false;
        }

        return true;
    }

    public bool LockTake(string cacheKey, int timeoutSeconds = 5, bool autoDelay = false)
    {
        var lockKey = CreateLockKey(cacheKey);
        var lockValue = cacheKey;
        var expiration = TimeSpan.FromSeconds(timeoutSeconds);
        var flag = _redisProvider.StringSet(lockKey, lockValue, expiration, StringSetWhen.NotExists);
        if (!flag || !autoDelay) return false;
        var refreshMilliseconds = (int)(expiration.TotalMilliseconds / 2.0);
        var renewalTimer = new Timer(RenewalTimerWorker,
            new RedisLockerState(lockKey, lockValue, (int)expiration.TotalMilliseconds), refreshMilliseconds,
            refreshMilliseconds);
        if (!AutoRenewalTimers.TryAdd(lockKey, renewalTimer))
        {
            renewalTimer.Dispose();
            return false;
        }

        return true;
    }

    public async Task<bool> LockReleaseAsync(string cacheKey)
    {
        var lockKey = CreateLockKey(cacheKey);
        if (AutoRenewalTimers.ContainsKey(lockKey))
            if (AutoRenewalTimers.TryRemove(lockKey, out var renewalTimer))
                await renewalTimer.DisposeAsync();

        var parameters = new { key = lockKey, value = cacheKey };
        var result = (int)await _redisProvider.EvalAsync(LuaDefaults.Unlock, parameters);
        return result == 1;
    }

    public bool LockRelease(string cacheKey)
    {
        var lockKey = CreateLockKey(cacheKey);
        if (AutoRenewalTimers.ContainsKey(lockKey))
            if (AutoRenewalTimers.TryRemove(lockKey, out var renewalTimer))
                renewalTimer.Dispose();

        var parameters = new { key = lockKey };
        var result = (int)_redisProvider.Eval(LuaDefaults.Unlock, parameters);
        return result == 1;
    }


    private string CreateLockKey(string cacheKey)
    {
        return string.IsNullOrWhiteSpace(_distributedLockerOptions?.KeyPrefix)
            ? $"{_prefix}_{cacheKey}"
            : $"{_distributedLockerOptions.KeyPrefix}_{cacheKey}";
    }

    private void RenewalTimerWorker(object? state)
    {
        if (state is RedisLockerState lockerState)
        {
            var lockKey = lockerState.LockKey;
            object parameters = new
            {
                key = lockKey,
                value = lockerState.LockValue,
                milliseconds = (int)lockerState.MilliSeconds
            };
            var lockResult = _redisProvider.Eval(LuaDefaults.AutoRenewalLock, parameters);
            Debug.WriteLine($"{DateTime.Now:yyyy-M-d dd HH:mm:ss}:lockResult:{lockResult}");
            if ((int)lockResult == 0)
                LockRelease(lockKey);
        }
    }
}