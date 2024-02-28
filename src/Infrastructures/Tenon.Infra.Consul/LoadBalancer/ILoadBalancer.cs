namespace Tenon.Infra.Consul.LoadBalancer;

public interface ILoadBalancer
{
    string Resolve(IReadOnlyCollection<string> services);
}