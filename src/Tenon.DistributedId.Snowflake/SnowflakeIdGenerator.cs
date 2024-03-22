using Tenon.DistributedId.Abstractions;
using Tenon.DistributedId.Snowflake.Exceptions;
using Yitter.IdGenerator;

namespace Tenon.DistributedId.Snowflake;

public sealed class SnowflakeIdGenerator : IDGenerator
{
    private static readonly object SyncRoot = new();

    private readonly IdGeneratorOptions _options = new()
    {
        WorkerIdBitLength = 6,
        SeqBitLength = 6
    };


    public void SetWorkerId(ushort workerId)
    {
        lock (SyncRoot)
        {
            _options.WorkerId = workerId;
            YitIdHelper.SetIdGenerator(_options);
            WorkerId = (short)workerId;
        }
    }

    public void ResetWorkerId()
    {
        lock (SyncRoot)
        {
            WorkerId = null;
        }
    }

    public long GetNextId()
    {
        if (!WorkerId.HasValue)
            throw new IdGeneratorException("Current workId is not available");
        return YitIdHelper.NextId();
    }

    public short? WorkerId { get; private set; } = null;

    public short MaxWorkerId => (short)(Math.Pow(2.0, _options.WorkerIdBitLength) - 1);
}