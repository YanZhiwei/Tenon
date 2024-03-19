using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Tenon.Infra.RabbitMq.Configurations;
using Tenon.Infra.RabbitMq.Models;

namespace Tenon.Infra.RabbitMq;

public abstract class RabbitMqProducer(
    RabbitMqConnection rabbitMqConnection,
    ILogger<RabbitMqProducer> logger)
{
    private static readonly object Lock = new();

    private readonly RabbitMqConnection _rabbitMqConnection =
        rabbitMqConnection ?? throw new ArgumentNullException(nameof(rabbitMqConnection));

    protected readonly ILogger<RabbitMqProducer> Logger = logger ?? throw new ArgumentNullException(nameof(logger));

    protected readonly RabbitMqOptions RabbitMqOptions =
        rabbitMqConnection?.Options ?? throw new ArgumentNullException(nameof(rabbitMqConnection));

    private RabbitMqBroker? _rabbitMqBroker;

    protected RabbitMqBroker RabbitMqBroker
    {
        get
        {
            if (_rabbitMqBroker == null)
                lock (Lock)
                {
                    if (_rabbitMqBroker == null)
                    {
                        _rabbitMqBroker = GetRabbitMqBroker();
                        _rabbitMqBroker.Declare();
                    }
                }

            return _rabbitMqBroker;
        }
    }

    protected abstract RabbitMqBroker GetRabbitMqBroker();

    public virtual void PublishBasicMessage(RabbitMqMessage rabbitMqMessage, IBasicProperties? basicProperties = null)
    {
        if (rabbitMqMessage == null)
            throw new ArgumentNullException(nameof(rabbitMqMessage));
        _rabbitMqConnection.TryConnect();
        using (var channel = _rabbitMqConnection.Connection.CreateModel())
        {
            channel.BasicPublish(RabbitMqBroker.ExchangeName, rabbitMqMessage.RoutingKey, basicProperties,
                rabbitMqMessage.Body);
            Logger.LogDebug(
                $"RabbitMq:[{RabbitMqOptions.Name}] publish message succeeded,routeKey:{rabbitMqMessage.RoutingKey}");
        }
    }
}