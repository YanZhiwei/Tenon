using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Tenon.DistributedId.Snowflake;

public sealed class SnowflakeWorkerNodeHostedService(
    ILogger<SnowflakeWorkerNodeHostedService> logger,
    WorkerNode workerNode)
    : BackgroundService
{
    private readonly ILogger<SnowflakeWorkerNodeHostedService> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly WorkerNode _workerNode = workerNode ?? throw new ArgumentNullException(nameof(workerNode));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.CompletedTask;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await _workerNode.RegisterAsync();
        _logger.LogDebug("SnowflakeWorkerNodeHostedService start succeeded.");
        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _workerNode.UnRegisterAsync();
        _logger.LogDebug("SnowflakeWorkerNodeHostedService stop succeeded.");
        await base.StopAsync(cancellationToken);
    }
}