namespace Tenon.Infra.Consul.Configurations;

public class ConsulDiscoveryOptions
{
    public string ConsulUrl { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
}