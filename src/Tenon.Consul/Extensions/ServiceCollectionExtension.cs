using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Tenon.Consul.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddConsul(this IServiceCollection services, IConfigurationSection consulSection)
    {
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
}