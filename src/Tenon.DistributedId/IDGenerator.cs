using System;

namespace Tenon.DistributedId
{
    public interface IDGenerator
    {
        void SetWorkerId(ushort workerId);

        long GetNextId();
    }
}
