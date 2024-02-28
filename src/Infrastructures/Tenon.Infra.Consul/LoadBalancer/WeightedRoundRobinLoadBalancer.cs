namespace Tenon.Infra.Consul.LoadBalancer;

internal sealed class WeightedRoundRobinLoadBalancer : ILoadBalancer
{
    private static readonly object SyncRoot = new();
    private int _index;

    public string Resolve(IReadOnlyCollection<string> services)
    {
        lock (SyncRoot)
        {
            var selectedServer = services.ElementAt(_index);
            _index = (_index + 1) % services.Count;
            return selectedServer;
        }
    }
}