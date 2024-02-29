namespace Tenon.Infra.RabbitMq.Models;

public sealed class RabbitMqExchange
{
    public RabbitMqExchange()
    {
        IsDurable = true;
        IsAutoDelete = false;
        Type = ExchangeType.Direct;
    }

    public string Name { get; set; }

    public ExchangeType Type { get; set; }

    public bool IsDurable { get; set; }

    public bool IsAutoDelete { get; set; }
}