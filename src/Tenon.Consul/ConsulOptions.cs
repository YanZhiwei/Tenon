namespace Tenon.Consul;

public sealed class ConsulOptions
{
    public string ConsulUrl { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string HealthCheckUrl { get; set; } = string.Empty;
    public int CheckIntervalInSecond { get; set; } = default;
    public string[] Tags { get; set; } = Array.Empty<string>();
    public string KeyPath { get; set; } = string.Empty;
    public int DeregisterCriticalServiceAfter { get; set; } = default;
    public int CheckTimeout { get; set; } = default;
}