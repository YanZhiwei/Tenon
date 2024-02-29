namespace Tenon.Infra.RabbitMq.Models;

public sealed class RabbitMqQueue
{
    public readonly string ExchangeName;
    public readonly bool Exclusive;
    public readonly bool IsAutoDelete;

    public readonly bool IsDurable;

    public readonly string Name;

    public readonly string RoutingKey;

    public RabbitMqQueue(string name, string exchangeName, string routingKey, bool isDurable = true,
        bool isAutoDelete = false, bool exclusive = false)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ExchangeName = exchangeName ?? throw new ArgumentNullException(nameof(exchangeName));
        RoutingKey = routingKey ?? throw new ArgumentNullException(nameof(routingKey));
        IsDurable = isDurable;
        IsAutoDelete = isAutoDelete;
        Exclusive = exclusive;
    }
}