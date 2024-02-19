using Consul;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Tenon.Consul.Configurations;
using Tenon.Helper.Internal;

namespace Tenon.Consul;

public sealed class RegistrationProvider(
    IOptions<ConsulOptions> consulOption,
    IHostApplicationLifetime hostApplicationLifetime)
{
    private readonly IOptions<ConsulOptions> _consulOption =
        consulOption ?? throw new ArgumentNullException(nameof(consulOption));

    private readonly IHostApplicationLifetime _hostApplicationLifetime =
        hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));


    public void Register(Func<Uri> getServiceAddressHandle, string? serviceId = null)
    {
        if (getServiceAddressHandle == null)
            throw new ArgumentNullException(nameof(getServiceAddressHandle));
        if (string.IsNullOrWhiteSpace(serviceId))
            serviceId = $"{_consulOption.Value.ServiceName}-{Convert.ToString(DateTime.UtcNow.Ticks, 16)}";

        _hostApplicationLifetime.ApplicationStarted.Register(() =>
        {
            using (var consulClient = new ConsulClient(x => x.Address = new Uri(_consulOption.Value.ConsulUrl)))
            {
                var serviceAddress = getServiceAddressHandle();
                if (serviceAddress == null)
                    throw new ArgumentNullException(nameof(serviceAddress));
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
                var result = consulClient.Agent.ServiceRegister(agentServiceRegistration).GetAwaiter().GetResult();
            }
        });
        _hostApplicationLifetime.ApplicationStopping.Register(
            () =>
            {
                using (var consulClient = new ConsulClient(x => x.Address = new Uri(_consulOption.Value.ConsulUrl)))
                {
                    var result = consulClient.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
                }
            });
    }
}