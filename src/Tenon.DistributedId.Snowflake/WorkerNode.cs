using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tenon.DistributedId.Abstractions;
using Tenon.DistributedId.Snowflake.Configurations;
using Tenon.DistributedId.Snowflake.Exceptions;
using Tenon.DistributedId.Snowflake.Models;
using Tenon.Infra.Redis;

namespace Tenon.DistributedId.Snowflake;

public class WorkerNode
{
    private readonly IDGenerator _idGenerator;
    private readonly ILogger<WorkerNode> _logger;
    private readonly SnowflakeIdOptions _options;
    private readonly IRedisProvider _redisProvider;
    private Timer? _renewalRefreshTimer;

    private const string RenewalWorkerScript = "if redis.call('GET', @key)==@value then redis.call('pexpire', @key, @milliseconds) return 1 end return 0";

    private const string UnRegisterWorkerScript =
        "local lock = redis.call('setnx',KEYS[1],ARGV[1]);if lock == 1 then redis.call('pexpire',KEYS[1],ARGV[2]);return 1;else return 0;end;";

    public WorkerNode(IDGenerator idGenerator, ILogger<WorkerNode> logger, IRedisProvider redisProvider,
        IOptionsMonitor<SnowflakeIdOptions> options)
    {
        _idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _redisProvider = redisProvider ?? throw new ArgumentNullException(nameof(redisProvider));
        _options = options?.CurrentValue ?? throw new ArgumentNullException(nameof(options));
        if (_options.WorkerNode == null)
            throw new ArgumentNullException(nameof(_options.WorkerNode));
        _options.WorkerNode.RefreshTimeInSeconds = (int)(_options.WorkerNode.ExpireTimeInSeconds / 2.0);
        CurrentWorkId = -1;
    }

    public int CurrentWorkId { get; private set; }

    public string WorkIdCacheKey { get; private set; }

    public async Task RegisterAsync()
    {
        _logger.LogDebug($"Service:{_options.ServiceName} starts registering work nodes");
        await CleanRenewalRefreshTimerAsync();
        var expiration = TimeSpan.FromSeconds(_options.WorkerNode.ExpireTimeInSeconds);
        _logger.LogDebug(
            $"Service:{_options.ServiceName} maxWorkerId:{_idGenerator.MaxWorkerId},expireTimeInSeconds:{_options.WorkerNode.ExpireTimeInSeconds} refreshTimeInSeconds:{_options.WorkerNode.RefreshTimeInSeconds}");
        for (ushort i = 0; i < _idGenerator.MaxWorkerId; i++)
        {
            try
            {
                WorkIdCacheKey = $"{_options.WorkerNode.Prefix}_{i}";

                var flag = await _redisProvider.StringSetAsync(WorkIdCacheKey, CurrentWorkId.ToString(),
                    expiration, StringSetWhen.NotExists);

                if (flag)
                {
                    CurrentWorkId = i;
                    _logger.LogInformation(
                        $"Service:{_options.ServiceName} successful registration of work node,workId={CurrentWorkId} and heartbeat initiated.");
                    _idGenerator.SetWorkerId(i);
                    _renewalRefreshTimer = new Timer(RenewalTimerWorker,
                        new WorkerNodeState(WorkIdCacheKey, CurrentWorkId.ToString(),
                            (int)expiration.TotalMilliseconds),
                        _options.WorkerNode.RefreshTimeInSeconds,
                        _options.WorkerNode.RefreshTimeInSeconds);
                }
                else
                {
                    _logger.LogDebug(
                        $"Service:{_options.ServiceName} worker node registration failed and continues to be attempted");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Service: {_options.ServiceName} worker node registration exception");
            }
        }

        if (CurrentWorkId == -1)
            throw new IdGeneratorWorkerNodeException($"Service: {_options.ServiceName} Worker node registration failed");
    }

    private async Task CleanRenewalRefreshTimerAsync()
    {
        if (_renewalRefreshTimer != null)
        {
            _idGenerator.ResetWorkerId();
            _logger.LogWarning(
                $"Service:{_options.ServiceName} workId:{CurrentWorkId} reset succeeded.");
            _logger.LogWarning(
                $"Service:{_options.ServiceName} workId:{CurrentWorkId} start cancelling the heartbeat.");
            await _renewalRefreshTimer.DisposeAsync();
            _logger.LogWarning($"Service:{_options.ServiceName} workId:{CurrentWorkId} heartbeat cancelled.");
        }
    }

    private void RenewalTimerWorker(object? state)
    {
        if (state is WorkerNodeState workerNode)
        {
            object parameters = new
            {
                key = workerNode.WorkerKey,
                value = workerNode.WorkerValue,
                milliseconds = (int)workerNode.MilliSeconds
            };
            var renewalWorkerResult = false;
            try
            {
                var renewalResult =
                    _redisProvider.Eval(
                        RenewalWorkerScript,
                        parameters);

                if ((int)renewalResult == 0)
                {
                    _logger.LogWarning(
                        $"Service: {_options.ServiceName},workIdCacheKey:{workerNode.WorkerKey} renewal failed.");
                    UnRegister(workerNode.WorkerKey);
                }
                else
                {
                    renewalWorkerResult = true;
                    _logger.LogDebug(
                        $"Service: {_options.ServiceName},workIdCacheKey:{workerNode.WorkerKey} renewal succeeded.");
                }
                if (renewalWorkerResult == false)
                {
                    RegisterAsync().ConfigureAwait(true).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"Service: {_options.ServiceName},workIdCacheKey:{workerNode.WorkerKey} renewal exception");
                if (ex is IdGeneratorWorkerNodeException)
                    throw;
            }
        }
    }

    private void UnRegister(string workIdCacheKey)
    {
        try
        {
            var parameters = new { key = workIdCacheKey };
            var resultCode = (int)_redisProvider.Eval(UnRegisterWorkerScript, parameters);
            var result = resultCode == 1;
            _logger.LogDebug(result
                ? $"Service: {_options.ServiceName},workIdCacheKey:{workIdCacheKey} unRegister succeeded"
                : $"Service: {_options.ServiceName},workIdCacheKey:{workIdCacheKey} unRegister failed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Service: {_options.ServiceName},workIdCacheKey:{workIdCacheKey} unRegister exception");
        }
    }
}