namespace Tenon.Consul.LoadBalancer;

internal sealed class RoundRobinLoadBalancer : ILoadBalancer
{
    private static readonly object SyncRoot = new();
    private int _index;

    public string Resolve(IReadOnlyCollection<string> services)
    {
        lock (SyncRoot)
        {
            if (_index >= services.Count)
                Interlocked.Exchange(ref _index, 0);
            return services.ElementAt(Interlocked.Increment(ref _index));
        }
    }
}