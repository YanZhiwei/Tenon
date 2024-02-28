using Tenon.DistributedId.Abstractions;
using Yitter.IdGenerator;

namespace Tenon.DistributedId.Snowflake;

public sealed class YitterIdGenerator : IDGenerator
{
    private static readonly object SyncRoot = new();
    private readonly byte _seqBitLength = 6;
    private readonly byte _workerIdBitLength = 6;
    public short CurrentWorkerId { get; private set; } = -1;

    public void SetWorkerId(ushort workerId)
    {
        lock (SyncRoot)
        {
            YitIdHelper.SetIdGenerator(new IdGeneratorOptions(workerId)
            {
                WorkerIdBitLength = _workerIdBitLength,
                SeqBitLength = _seqBitLength
            });

            CurrentWorkerId = (short)workerId;
        }
    }

    public long GetNextId()
    {
        return YitIdHelper.NextId();
    }
}