using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tenon.Infra.Consul.Configurations;
using Tenon.Infra.Consul.Extensions;
using Xunit;

namespace Tenon.Infra.ConsulTests;

/// <summary>
/// 验证 AddConsul、AddConsulDiscovery 的 DI 注册与 null 校验。
/// </summary>
public sealed class ServiceCollectionExtensionTests
{
    [Fact]
    public void AddConsul_ThrowsArgumentNullException_WhenConsulSectionIsNull()
    {
        var services = new ServiceCollection();

        var ex = Assert.Throws<ArgumentNullException>(() =>
            services.AddConsul(null!));

        Assert.Equal("consulSection", ex.ParamName);
    }

    [Fact]
    public void AddConsul_BindsConsulOptions_WhenSectionProvided()
    {
        var configData = new Dictionary<string, string?>
        {
            ["Consul:ConsulUrl"] = "http://localhost:8500",
            ["Consul:ServiceName"] = "test-service",
            ["Consul:HealthCheckUrl"] = "health",
            ["Consul:CheckIntervalInSecond"] = "10",
            ["Consul:DeregisterCriticalServiceAfter"] = "30",
            ["Consul:CheckTimeout"] = "5",
            ["Consul:KeyPath"] = "app1"
        };
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();
        var section = config.GetSection("Consul");

        var services = new ServiceCollection()
            .AddConsul(section);

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<ConsulOptions>>().Value;

        Assert.Equal("http://localhost:8500", options.ConsulUrl);
        Assert.Equal("test-service", options.ServiceName);
        Assert.Equal("health", options.HealthCheckUrl);
        Assert.Equal(10, options.CheckIntervalInSecond);
        Assert.Equal(30, options.DeregisterCriticalServiceAfter);
        Assert.Equal(5, options.CheckTimeout);
        Assert.Equal("app1", options.KeyPath);
    }

    [Fact]
    public void AddConsulDiscovery_ThrowsArgumentNullException_WhenConsulDiscoverySectionIsNull()
    {
        var services = new ServiceCollection();

        var ex = Assert.Throws<ArgumentNullException>(() =>
            services.AddConsulDiscovery(null!));

        Assert.Equal("consulDiscoverySection", ex.ParamName);
    }
}
