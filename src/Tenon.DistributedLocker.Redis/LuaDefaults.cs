namespace Tenon.DistributedLocker.Redis;

internal sealed class LuaDefaults
{
    public static readonly string InitLock =
        "local lock = redis.call('setnx',KEYS[1],ARGV[1]);if lock == 1 then redis.call('pexpire',KEYS[1],ARGV[2]);return 1;else return 0;end;";

    public static readonly string TryLock =
        "local lock = redis.call('setnx',KEYS[1],ARGV[1]);if lock == 1 then redis.call('pexpire',KEYS[1],ARGV[2]);return 1;else return 0;end;";

    public static readonly string SetLock = "redis.call('pexpire',KEYS[1],ARGV[1]);return 1;";

    public static readonly string AutoRenewalLock = "if redis.call('GET', @key)==@value then redis.call('pexpire', @key, @milliseconds) return 1 end return 0";

    public static readonly string Unlock = "if redis.call('GET', @key)==@value then redis.call('del', @key) return 1 end return 0";
}