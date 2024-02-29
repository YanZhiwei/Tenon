namespace Tenon.Infra.RabbitMq.Configurations;

public sealed class RabbitMqOptions
{
    public string VirtualHost { get; set; }
    public string Name { get; set; }
    public string HostName { get; set; }
    public int Port { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool AutoReConnect { get; set; }
}