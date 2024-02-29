using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Tenon.Infra.RabbitMq.Configurations;
using Tenon.Infra.RabbitMq.Models;

namespace Tenon.Infra.RabbitMq;

public sealed class RabbitMqProducer(RabbitMqConnection rabbitMqConnection, ILogger<RabbitMqProducer> logger)
{
    private readonly ILogger<RabbitMqProducer> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly RabbitMqConnection _rabbitMqConnection =
        rabbitMqConnection ?? throw new ArgumentNullException(nameof(rabbitMqConnection));

    private readonly RabbitMqOptions _rabbitMqOptions =
        rabbitMqConnection?.Options ?? throw new ArgumentNullException(nameof(rabbitMqConnection));

    public void PublishBasicMessage(RabbitMqMessage rabbitMqMessage)
    {
        if (rabbitMqMessage == null)
            throw new ArgumentNullException(nameof(rabbitMqMessage));
        _rabbitMqConnection.TryConnect();
        using (var channel = CreateModel())
        {
            if (rabbitMqMessage.ChannelDeclare)
            {
                var rabbitMqChannel = rabbitMqMessage.Channel;
                var rabbitMqExchange = rabbitMqChannel.Exchange;

                channel.ExchangeDeclare(rabbitMqExchange.Name, rabbitMqExchange.Type.ToString().ToLowerInvariant(),
                    rabbitMqExchange.IsDurable, rabbitMqExchange.IsAutoDelete);
                _logger.LogDebug(
                    $"RabbitMq:[{_rabbitMqOptions.Name}] exchangeName:{rabbitMqExchange.Name} type:{rabbitMqExchange.Type} declare succeeded.");
                if (rabbitMqChannel.Queues?.Any() ?? false)
                    foreach (var rabbitMqQueue in rabbitMqChannel.Queues)
                    {
                        channel.QueueDeclare(rabbitMqQueue.Name,
                            rabbitMqQueue.IsDurable,
                            rabbitMqQueue.Exclusive,
                            rabbitMqQueue.IsAutoDelete,
                            null);
                        _logger.LogDebug(
                            $"RabbitMq:[{_rabbitMqOptions.Name}] queueName: {rabbitMqQueue.Name} declare succeeded.");
                        channel.QueueBind(rabbitMqQueue.Name, rabbitMqQueue.ExchangeName, rabbitMqQueue.RoutingKey);
                        _logger.LogDebug(
                            $"RabbitMq:[{_rabbitMqOptions.Name}] queueName {rabbitMqQueue.Name},routeKey:{rabbitMqQueue.RoutingKey} bind succeeded.");
                    }
                _logger.LogDebug(
                    $"RabbitMq:[{_rabbitMqOptions.Name}] channelName:{rabbitMqChannel.Name} declare succeeded.");
            }

            channel.BasicPublish(rabbitMqMessage.ExchangeName, rabbitMqMessage.RoutingKey, null, rabbitMqMessage.Body);
        }
    }

    private IModel CreateModel()
    {
        _rabbitMqConnection.TryConnect();
        if (!_rabbitMqConnection.IsConnected || _rabbitMqConnection.Connection == null)
            throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
        return _rabbitMqConnection.Connection.CreateModel();
    }
}