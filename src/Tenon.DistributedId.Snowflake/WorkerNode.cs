using Microsoft.Extensions.Logging;
using Tenon.DistributedId.Abstractions;
using Tenon.DistributedLocker.Abstractions;
using Tenon.Infra.Redis;

namespace Tenon.DistributedId.Snowflake;

public sealed class WorkerNode
{
    private readonly IDistributedLocker _distributedLocker;
    private readonly int _expireTimeInSeconds;
    private readonly IDGenerator _idGenerator;
    private readonly ILogger<WorkerNode> _logger;
    private readonly IRedisProvider _redisProvider;
    private readonly string _workerRegisterCacheKey;
    public readonly string ServiceName;

    public WorkerNode(IDGenerator idGenerator, ILogger<WorkerNode> logger, IRedisProvider redisProvider,
        IDistributedLocker distributedLocker, string serviceName,
        string workerRegisterCacheKey = "snowflake:$serviceName:workids", int expireTimeInSeconds = 100)
    {
        _idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _redisProvider = redisProvider ?? throw new ArgumentNullException(nameof(redisProvider));
        _distributedLocker = distributedLocker ?? throw new ArgumentNullException(nameof(distributedLocker));
        ServiceName = serviceName ?? throw new ArgumentNullException(nameof(serviceName));
        _workerRegisterCacheKey = workerRegisterCacheKey.Replace("$serviceName", ServiceName);
        _expireTimeInSeconds = expireTimeInSeconds;
    }

    public async Task RegisterAsync()
    {
        for (ushort i = 0; i < _idGenerator.MaxWorkerId; i++)
        {
            var workIdCacheKey = $"{_workerRegisterCacheKey}_{i}";
            try
            {
                if (await _distributedLocker.LockTakeAsync(workIdCacheKey, _expireTimeInSeconds))
                {
                    _idGenerator.SetWorkerId(i);
                    break;
                }
            }
            finally
            {
                await _distributedLocker.LockReleaseAsync(_workerRegisterCacheKey);
            }
        }
    }

    public async Task RefreshAsync()
    {
        if (_idGenerator.WorkerId == -1) throw new InvalidOperationException("WorkId not yet registered");

        await Task.CompletedTask;
    }
}