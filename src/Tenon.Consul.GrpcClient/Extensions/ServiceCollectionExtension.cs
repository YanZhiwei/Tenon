using Grpc.Core;
using Grpc.Net.Client.Balancer;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tenon.Consul.Configurations;
using Tenon.Consul.GrpcClient.Configurations;
using Tenon.Consul.LoadBalancer;

namespace Tenon.Consul.GrpcClient.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddConsulRestClient<TGrpcClient>(this IServiceCollection services,
        IConfigurationSection consulGrpcClientSection, ILoadBalancer? loadBalancer = null)
        where TGrpcClient : class
    {
        if (consulGrpcClientSection == null)
            throw new ArgumentNullException(nameof(consulGrpcClientSection));
        var consulGrpcClientOptions = consulGrpcClientSection.Get<ConsulGrpcClientOptions>();
        if (consulGrpcClientOptions == null)
            throw new ArgumentNullException(nameof(consulGrpcClientOptions));
        loadBalancer ??= new RandomLoadBalancer();
        services.AddGrpcClient<TGrpcClient>(options => options.Address = new Uri(consulGrpcClientOptions.ConsulUrl))
            .ConfigureChannel(options =>
            {
                options.Credentials = ChannelCredentials.Insecure;
                options.ServiceConfig = new ServiceConfig { LoadBalancingConfigs = { new RoundRobinConfig() } };
            });
        return services.Configure<ConsulGrpcClientOptions>(consulGrpcClientSection)
            .Configure<ConsulDiscoveryOptions>(consulGrpcClientSection)
            .AddSingleton(loadBalancer)
            .AddSingleton<ResolverFactory, ConsulGrpcResolverFactory>()
            .AddScoped<ConsulDiscoverDelegatingHandler>();
    }
}