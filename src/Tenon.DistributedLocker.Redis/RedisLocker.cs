using System.Diagnostics;

namespace Tenon.DistributedLocker.Redis;

internal sealed class RedisLocker
{
    private static readonly string Prefix;

    static RedisLocker()
    {
        using (var currentProcess = Process.GetCurrentProcess())
        {
            Prefix = $"{Environment.MachineName}_{currentProcess.Id}";
        }
    }

    public static string CreateLockId()
    {
        return $"{Prefix}_{Guid.NewGuid():N}";
        
    }
}