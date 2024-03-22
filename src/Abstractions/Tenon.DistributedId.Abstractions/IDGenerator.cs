namespace Tenon.DistributedId.Abstractions
{
    public interface IDGenerator
    {
        void SetWorkerId(ushort workerId);

        void ResetWorkerId();

        long GetNextId();

        short? WorkerId { get; }

        short MaxWorkerId { get; }
    }
}
