using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Tenon.Infra.RabbitMq.Configurations;
using Tenon.Infra.RabbitMq.Models;

namespace Tenon.Infra.RabbitMq;

public abstract class RabbitMqConsumer : IDisposable
{
    private readonly IModel _channel;
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly RabbitMqBroker _rabbitMqBroker;
    private readonly RabbitMqConnection _rabbitMqConnection;
    private readonly RabbitMqOptions _rabbitMqOptions;

    protected RabbitMqConsumer(RabbitMqConnection rabbitMqConnection,
        RabbitMqBroker rabbitMqBroker,
        ILogger<RabbitMqConsumer> logger)
    {
        _rabbitMqBroker = rabbitMqBroker ?? throw new ArgumentNullException(nameof(rabbitMqBroker));
        _rabbitMqConnection = rabbitMqConnection ?? throw new ArgumentNullException(nameof(rabbitMqConnection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _channel = _rabbitMqConnection.Connection.CreateModel();
        _rabbitMqOptions = rabbitMqConnection.Options;
    }

    public void Dispose()
    {
        _rabbitMqConnection.Dispose();
    }

    protected abstract Task<bool> BasicConsumerReceived(string exchange, string routingKey, byte[] body,
        RabbitMqQueue queue);

    public void BasicConsumer(params string[] routeKeys)
    {
        if ((routeKeys?.Any() ?? false) == false)
            throw new ArgumentNullException(nameof(routeKeys));


        var queues = _rabbitMqBroker.Declare(_channel, routeKeys)?.ToArray();
        if (queues?.Any() ?? false)
        {
            var consumer = new EventingBasicConsumer(_channel);
            foreach (var queue in queues)
            {
                _channel.BasicConsume(queue.Name, queue.AutoAck, queue.ConsumerTag, false, queue.Exclusive,
                    queue.Arguments, consumer);
                _logger.LogDebug(
                    $"RabbitMq:[{_rabbitMqOptions.Name}] queueName:{queue.Name} consumer succeeded,routeKey:{queue.RoutingKey}");
            }

            var dic = queues.ToDictionary(item => item.RoutingKey, item => item);

            consumer.Received += async (model, ea) =>
            {
                _logger.LogDebug($"RabbitMq:[{_rabbitMqOptions.Name}] message received,routeKey:{ea.RoutingKey}");
                var queue = dic[ea.RoutingKey];
                var result = await BasicConsumerReceived(ea.Exchange, ea.RoutingKey, ea.Body.ToArray(), queue);
                if (queue.AutoAck) return;
                if (result)
                    _channel.BasicAck(ea.DeliveryTag, queue.AckMultiple);
                else
                    _channel.BasicReject(ea.DeliveryTag, queue.RejectRequeue);
            };
        }
    }
}