namespace Tenon.Consul.LoadBalancer;

internal sealed class IpHashLoadBalancer : ILoadBalancer
{
    public string Resolve(IReadOnlyCollection<string> services)
    {
        throw new NotImplementedException();
    }
}