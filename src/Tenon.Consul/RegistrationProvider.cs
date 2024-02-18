using Consul;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Tenon.Consul.Options;

namespace Tenon.Consul;

public sealed class RegistrationProvider(
    IOptions<ConsulOptions> consulOption,
    ConsulClient consulClient,
    IHostApplicationLifetime hostApplicationLifetime,
    IServer server)
{
    private readonly ConsulClient _consulClient = consulClient ?? throw new ArgumentNullException(nameof(consulClient));

    private readonly IOptions<ConsulOptions> _consulOption =
        consulOption ?? throw new ArgumentNullException(nameof(consulOption));

    private readonly IHostApplicationLifetime _hostApplicationLifetime =
        hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));

    private readonly IServer _server = server ?? throw new ArgumentNullException(nameof(server));

    public void Register(string? serviceId = null)
    {
        if (string.IsNullOrWhiteSpace(serviceId))
            serviceId = $"{_consulOption.Value.ServiceName}-{Convert.ToString(DateTime.UtcNow.Ticks, 16)}";
        _hostApplicationLifetime.ApplicationStarted.Register(() =>
        {
            var addresses = _server.Features.Get<IServerAddressesFeature>().Addresses;
            var address = addresses.FirstOrDefault();
            var serviceAddress = new Uri(address);
            var protocol = serviceAddress.Scheme;
            var host = serviceAddress.Host;
            var port = serviceAddress.Port;
            var agentServiceRegistration = new AgentServiceRegistration
            {
                ID = serviceId,
                Name = _consulOption.Value.ServiceName,
                Address = host,
                Port = port,
                Meta = new Dictionary<string, string> { ["Protocol"] = protocol },
                Tags = _consulOption.Value.Tags,
                Check = new AgentServiceCheck
                {
                    DeregisterCriticalServiceAfter =
                        TimeSpan.FromSeconds(_consulOption.Value.DeregisterCriticalServiceAfter),
                    Interval = TimeSpan.FromSeconds(_consulOption.Value.CheckIntervalInSecond),
                    HTTP = $"{protocol}://{host}:{port}/{_consulOption.Value.HealthCheckUrl}",
                    Timeout = TimeSpan.FromSeconds(_consulOption.Value.CheckTimeout)
                }
            };
            _consulClient.Agent.ServiceRegister(agentServiceRegistration);
        });
        _hostApplicationLifetime.ApplicationStopping.Register(() =>
            _consulClient.Agent.ServiceDeregister(serviceId));
    }
}