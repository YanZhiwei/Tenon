using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tenon.Infra.Consul.Configurations;
using Tenon.Infra.Consul.LoadBalancer;

namespace Tenon.Infra.Consul.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddConsul(this IServiceCollection services, IConfigurationSection consulSection)
    {
        if (consulSection == null)
            throw new ArgumentNullException(nameof(consulSection));
        return services.Configure<ConsulOptions>(consulSection);
    }

    public static IServiceCollection AddConsulDiscovery(this IServiceCollection services,
        IConfigurationSection consulDiscoverySection)
    {
        if (consulDiscoverySection == null)
            throw new ArgumentNullException(nameof(consulDiscoverySection));
        return services
            .Configure<ConsulDiscoveryOptions>(consulDiscoverySection)
            .AddSingleton<ILoadBalancer, RandomLoadBalancer>()
            .AddScoped<ConsulDiscoverDelegatingHandler>();
    }
}