using System.Collections.Concurrent;
using Tenon.Redis;

namespace Tenon.DistributedLocker.Redis;

public sealed class RedisDistributedLocker(IRedisProvider redisProvider) : IDistributedLocker
{
    private static readonly ConcurrentDictionary<string, RedisLockerTimer> AutoDelayTimers;
    private readonly IRedisProvider _redisProvider = redisProvider ?? throw new ArgumentNullException(nameof(redisProvider));

    static RedisDistributedLocker()
    {
        AutoDelayTimers = new ConcurrentDictionary<string, RedisLockerTimer>();
    }

    public async Task<bool> LockAsync(string cacheKey, int timeoutSeconds = 5, bool autoDelay = false)
    {
        var redisLockerTimer = new RedisLockerTimer(cacheKey, AutoDelayTimerWorker, timeoutSeconds);
        var lockKey = redisLockerTimer.LockKeyId;
        var lockValue = redisLockerTimer.LockValue;
        var expiration = redisLockerTimer.Expiration;
        var flag = await _redisProvider.StringSetAsync(lockKey, lockValue, expiration, StringSetWhen.NotExists);
        if (flag && autoDelay)
        {
            var addResult = AutoDelayTimers.TryAdd(lockKey, redisLockerTimer);
            if (!addResult)
            {
                //
            }

            return addResult;
        }

        return false;
    }


    private void AutoDelayTimerWorker(object? state)
    {
        if (state is RedisLockerState lockerState)
        {
            object parameters = new
                { key = lockerState.LockKeyId, value = lockerState.LockValue, milliseconds = lockerState.Milliseconds };
            var lockResult = _redisProvider.Eval(LuaDefaults.DelayLock, parameters);
            if ((int)lockResult == 0) Close(lockerState.LockValue);
        }
    }

    private void Close(string key)
    {
        if (AutoDelayTimers.ContainsKey(key))
            if (AutoDelayTimers.TryRemove(key, out var timer))
                timer?.Dispose();
    }
}