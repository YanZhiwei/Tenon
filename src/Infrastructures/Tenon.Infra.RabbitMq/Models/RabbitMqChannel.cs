namespace Tenon.Infra.RabbitMq.Models;

public sealed class RabbitMqChannel
{
    public readonly bool Global;

    public readonly int PrefetchCount;
    public readonly int PrefetchSize;

    public RabbitMqChannel(int prefetchSize = 0, int prefetchCount = 1, bool global = true)
    {
        PrefetchSize = prefetchSize;
        PrefetchCount = prefetchCount;
        Global = global;
    }
}