using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tenon.Consul.Options;

namespace Tenon.Consul.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddConsul(this IServiceCollection services, IConfigurationSection consulSection)
    {
        if (consulSection == null)
            throw new ArgumentNullException(nameof(consulSection));
        return services
                .Configure<ConsulOptions>(consulSection)
                .AddSingleton(provider =>
                {
                    var configOptions = provider.GetService<IOptions<ConsulOptions>>();
                    if (configOptions == null)
                        throw new NullReferenceException(nameof(configOptions));
                    return new ConsulClient(x => x.Address = new Uri(configOptions.Value.ConsulUrl));
                })
            ;
    }

    public static IServiceCollection AddConsulDiscovery(this IServiceCollection services, IConfigurationSection consulDiscoverySection)
    {
        if (consulDiscoverySection == null)
            throw new ArgumentNullException(nameof(consulDiscoverySection));
        return services
            .Configure<ConsulDiscoveryOptions>(consulDiscoverySection);
    }
}