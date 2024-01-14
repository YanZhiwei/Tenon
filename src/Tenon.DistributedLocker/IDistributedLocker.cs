using System;
using System.Threading.Tasks;

namespace Tenon.DistributedLocker
{
    public interface IDistributedLocker
    {
        Task<bool> LockTakeAsync(string cacheKey, int timeoutSeconds = 5, bool autoDelay = false);
        bool LockTake(string cacheKey, int timeoutSeconds = 5, bool autoDelay = false);
        Task<bool> LockReleaseAsync(string cacheKey);
        bool LockRelease(string cacheKey);

    }
}