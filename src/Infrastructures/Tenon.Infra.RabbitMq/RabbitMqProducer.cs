using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Tenon.Infra.RabbitMq.Configurations;
using Tenon.Infra.RabbitMq.Models;

namespace Tenon.Infra.RabbitMq;

public sealed class RabbitMqProducer(
    RabbitMqConnection rabbitMqConnection,
    RabbitMqBroker rabbitMqBroker,
    ILogger<RabbitMqProducer> logger)
{
    private readonly ILogger<RabbitMqProducer> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly RabbitMqBroker _rabbitMqBroker =
        rabbitMqBroker ?? throw new ArgumentNullException(nameof(rabbitMqBroker));

    private readonly RabbitMqConnection _rabbitMqConnection =
        rabbitMqConnection ?? throw new ArgumentNullException(nameof(rabbitMqConnection));

    private readonly RabbitMqOptions _rabbitMqOptions =
        rabbitMqConnection?.Options ?? throw new ArgumentNullException(nameof(rabbitMqConnection));

    public void PublishBasicMessage(RabbitMqMessage rabbitMqMessage, IBasicProperties? basicProperties = null)
    {
        if (rabbitMqMessage == null)
            throw new ArgumentNullException(nameof(rabbitMqMessage));
        using (var channel = _rabbitMqConnection.Connection.CreateModel())
        {
            channel.BasicPublish(_rabbitMqBroker.ExchangeName, rabbitMqMessage.RoutingKey, basicProperties,
                rabbitMqMessage.Body);
            _logger.LogDebug($"RabbitMq:[{_rabbitMqOptions.Name}] publish message succeeded,routeKey:{rabbitMqMessage.RoutingKey}");
        }
    }
}