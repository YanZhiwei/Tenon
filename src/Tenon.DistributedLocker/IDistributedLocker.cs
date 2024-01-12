using System;
using System.Threading.Tasks;

namespace Tenon.DistributedLocker
{
    public interface IDistributedLocker
    {
        Task<bool> LockAsync(string cacheKey, int timeoutSeconds = 5, bool autoDelay = false);
    }
}
