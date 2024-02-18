using Consul;
using Tenon.Consul.LoadBalancer;
using Tenon.Consul.Options;

namespace Tenon.Consul;

public sealed class ServiceDiscoveryProvider(ConsulOptions consulOptions, ILoadBalancer loadBalancer)
{
    private readonly ConsulOptions _consulOptions =
        consulOptions ?? throw new ArgumentNullException(nameof(consulOptions));

    private ILoadBalancer LoadBalancer { get; } = loadBalancer ?? throw new ArgumentNullException(nameof(loadBalancer));

    public async Task<IReadOnlyList<string>?> GetAllHealthServicesAsync()
    {
        using (var consulClient =
               new ConsulClient(c => c.Address = new Uri(_consulOptions.ConsulUrl)))
        {
            var query = await consulClient.Health.Service(_consulOptions.ServiceName, string.Empty, true);
            if (query is not null && (query.Response?.Any() ?? false))
                return query.Response.Select(entry => $"{entry.Service.Address}:{entry.Service.Port}").ToList();
            return null;
        }
    }

    public async Task<string> GetSingleHealthServiceAsync()
    {
        var services = await GetAllHealthServicesAsync();
        if (services is null)
            throw new InvalidOperationException($"No service:{_consulOptions.ServiceName} found");
        return LoadBalancer.Resolve(services);
    }
}