namespace Tenon.DistributedId.Abstractions
{
    public interface IDGenerator
    {
        void SetWorkerId(ushort workerId);

        long GetNextId();
    }
}
