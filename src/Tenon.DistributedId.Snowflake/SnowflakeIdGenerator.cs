using Tenon.DistributedId.Abstractions;
using Yitter.IdGenerator;

namespace Tenon.DistributedId.Snowflake;

public sealed class SnowflakeIdGenerator : IDGenerator
{
    private static readonly object SyncRoot = new();
    private readonly IdGeneratorOptions _options;

    public SnowflakeIdGenerator()
    {
        _options = new IdGeneratorOptions
        {
            WorkerIdBitLength = 6,
            SeqBitLength = 6
        };
    }


    public void SetWorkerId(ushort workerId)
    {
        lock (SyncRoot)
        {
            _options.WorkerId = workerId;
            YitIdHelper.SetIdGenerator(_options);

            WorkerId = (short)workerId;
        }
    }

    public long GetNextId()
    {
        return YitIdHelper.NextId();
    }

    public short WorkerId { get; private set; } = -1;

    public short MaxWorkerId => (short)(Math.Pow(2.0, _options.WorkerIdBitLength) - 1);
}