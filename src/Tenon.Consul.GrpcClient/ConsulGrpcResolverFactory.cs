using Grpc.Net.Client.Balancer;
using Microsoft.Extensions.Options;
using Tenon.Consul.GrpcClient.Configurations;
using Tenon.Consul.LoadBalancer;

namespace Tenon.Consul.GrpcClient;

public sealed class ConsulGrpcResolverFactory(
    IOptions<ConsulGrpcClientOptions> consulGrpcClientOptions,
    ILoadBalancer loadBalancer)
    : ResolverFactory
{
    private readonly IOptions<ConsulGrpcClientOptions> _consulGrpcClientOptions =
        consulGrpcClientOptions ?? throw new ArgumentNullException(nameof(consulGrpcClientOptions));

    private readonly ILoadBalancer
        _loadBalancer = loadBalancer ?? throw new ArgumentNullException(nameof(loadBalancer));

    public override string Name => "consul";

    public override Resolver Create(ResolverOptions options)
    {
        return new ConsulGrpcResolver(_consulGrpcClientOptions, _loadBalancer, options.LoggerFactory);
    }
}