namespace Tenon.Infra.RabbitMq.Models;

public sealed class RabbitMqExchange
{
    public RabbitMqExchange()
    {
        Durable = true;
        AutoDelete = false;
        Type = ExchangeType.Direct;
    }

    public string Name { get; set; }

    public ExchangeType Type { get; set; }

    public bool Durable { get; set; }

    public bool AutoDelete { get; set; }
}