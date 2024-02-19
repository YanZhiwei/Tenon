using Grpc.Net.Client.Balancer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tenon.Consul.GrpcClient.Configurations;
using Tenon.Consul.LoadBalancer;

namespace Tenon.Consul.GrpcClient;

public sealed class ConsulGrpcResolver : PollingResolver
{
    private readonly ConsulGrpcClientOptions _consulGrpcClientOptions;
    private readonly ILogger _logger;
    private readonly TimeSpan _refreshInterval;
    private readonly ServiceDiscoveryProvider _serviceDiscoveryProvider;
    private Timer? _refreshTimer;

    public ConsulGrpcResolver(IOptions<ConsulGrpcClientOptions> consulGrpcClientOptions,
        ILoadBalancer loadBalancer, ILoggerFactory loggerFactory) : base(loggerFactory)
    {
        _consulGrpcClientOptions = consulGrpcClientOptions?.Value ??
                                   throw new ArgumentNullException(nameof(consulGrpcClientOptions));
        loadBalancer = loadBalancer ?? throw new ArgumentNullException(nameof(loadBalancer));
        _logger = loggerFactory.CreateLogger<ConsulGrpcResolver>();
        _serviceDiscoveryProvider = new ServiceDiscoveryProvider(consulGrpcClientOptions, loadBalancer);
        _refreshInterval = TimeSpan.FromSeconds(30);
    }


    protected override async Task ResolveAsync(CancellationToken cancellationToken)
    {
        var healthServices = await _serviceDiscoveryProvider.GetAllHealthServicesAsync(cancellationToken);

        if (healthServices?.Count == 0)
        {
            _logger.LogWarning(
                $"[{_consulGrpcClientOptions.ConsulUrl}:{_consulGrpcClientOptions.ServiceName}] No healthy service found");
            return;
        }

        if (healthServices != null)
        {
            var balancerAddresses = new List<BalancerAddress>();
            foreach (var service in healthServices)
            {
                var addressArray = service.Split(":");
                var host = addressArray[0];
                var port = int.Parse(addressArray[1]) + 1;
                balancerAddresses.Add(new BalancerAddress(host, port));
            }

            Listener(ResolverResult.ForResult(balancerAddresses));
        }
    }

    protected override void OnStarted()
    {
        base.OnStarted();
        if (_refreshInterval != Timeout.InfiniteTimeSpan)
        {
            _refreshTimer = new Timer(OnTimerCallback, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _refreshTimer.Change(_refreshInterval, _refreshInterval);
        }
    }

    private void OnTimerCallback(object? state)
    {
        try
        {
            Refresh();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when refreshing the service list");
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _refreshTimer?.Dispose();
    }
}