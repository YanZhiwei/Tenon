using Consul;
using Tenon.Consul.LoadBalancer;
using Tenon.Consul.Options;

namespace Tenon.Consul;

public sealed class ServiceDiscoveryProvider(ConsulDiscoveryOptions consulDiscoveryOptions, ILoadBalancer loadBalancer)
{
    private readonly ConsulDiscoveryOptions _consulDiscoveryOptions =
        consulDiscoveryOptions ?? throw new ArgumentNullException(nameof(consulDiscoveryOptions));

    private ILoadBalancer LoadBalancer { get; } = loadBalancer ?? throw new ArgumentNullException(nameof(loadBalancer));

    public async Task<IReadOnlyList<string>?> GetAllHealthServicesAsync()
    {
        using (var consulClient =
               new ConsulClient(c => c.Address = new Uri(_consulDiscoveryOptions.ConsulUrl)))
        {
            var query = await consulClient.Health.Service(_consulDiscoveryOptions.ServiceName, string.Empty, true);
            if (query is not null && (query.Response?.Any() ?? false))
                return query.Response.Select(entry => $"{entry.Service.Address}:{entry.Service.Port}").ToList();
            return null;
        }
    }

    public async Task<string> GetSingleHealthServiceAsync()
    {
        var services = await GetAllHealthServicesAsync();
        if (services is null)
            throw new InvalidOperationException($"No service:{_consulDiscoveryOptions.ServiceName} found");
        return LoadBalancer.Resolve(services);
    }
}