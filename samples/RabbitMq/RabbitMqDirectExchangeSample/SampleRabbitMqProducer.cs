using Microsoft.Extensions.Logging;
using Tenon.Infra.RabbitMq;
using Tenon.Infra.RabbitMq.Models;

namespace RabbitMqDirectExchangeSample;

public sealed class SampleRabbitMqProducer(RabbitMqConnection rabbitMqConnection, ILogger<RabbitMqProducer> logger)
    : RabbitMqProducer(rabbitMqConnection, logger)
{
    protected override RabbitMqBroker GetRabbitMqBroker()
    {
        var exchange = new RabbitMqExchange
        {
            Name = "MyExchange",
            Type = ExchangeType.Direct
        };

        var queues = new List<RabbitMqQueue>
        {
            new("log_all", exchange.Name, "debug"),
            new("log_all", exchange.Name, "info"),
            new("log_all", exchange.Name, "warn"),
            new("log_all", exchange.Name, "error"),
            new("log_error", exchange.Name, "error")
        };
        return new RabbitMqBroker(rabbitMqConnection, exchange, queues, Logger);
    }
}