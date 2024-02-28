using Consul;
using Microsoft.Extensions.Options;
using Tenon.Infra.Consul.Configurations;
using Tenon.Infra.Consul.LoadBalancer;

namespace Tenon.Infra.Consul;

public sealed class ServiceDiscoveryProvider(IOptions<ConsulDiscoveryOptions> consulDiscoveryOptions, ILoadBalancer loadBalancer)
{
    private readonly ConsulDiscoveryOptions _consulDiscoveryOptions =
        consulDiscoveryOptions?.Value ?? throw new ArgumentNullException(nameof(consulDiscoveryOptions));

    private readonly ILoadBalancer _loadBalancer = loadBalancer ?? throw new ArgumentNullException(nameof(loadBalancer));

    public async Task<IReadOnlyList<string>?> GetAllHealthServicesAsync(CancellationToken ct = default(CancellationToken))
    {
        using (var consulClient =
               new ConsulClient(c => c.Address = new Uri(_consulDiscoveryOptions.ConsulUrl)))
        {
            var query = await consulClient.Health.Service(_consulDiscoveryOptions.ServiceName, string.Empty, true, ct);
            if (query is not null && (query.Response?.Any() ?? false))
                return query.Response.Select(entry => $"{entry.Service.Address}:{entry.Service.Port}").ToList();
            return null;
        }
    }

    public async Task<string> GetSingleHealthServiceAsync(CancellationToken ct = default(CancellationToken))
    {
        var services = await GetAllHealthServicesAsync(ct);
        if (services is null)
            throw new InvalidOperationException($"No service:{_consulDiscoveryOptions.ServiceName} found");
        return _loadBalancer.Resolve(services);
    }
}