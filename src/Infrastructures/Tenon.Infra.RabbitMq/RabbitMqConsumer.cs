using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Tenon.Infra.RabbitMq.Configurations;
using Tenon.Infra.RabbitMq.Extensions;
using Tenon.Infra.RabbitMq.Models;

namespace Tenon.Infra.RabbitMq;

public abstract class RabbitMqConsumer : IDisposable
{
    private static readonly object Lock = new();
    private readonly IModel _channel;
    protected readonly ILogger<RabbitMqConsumer> Logger;
    protected readonly RabbitMqConnection RabbitMqConnection;
    protected readonly RabbitMqOptions RabbitMqOptions;

    private RabbitMqBroker? _rabbitMqBroker;

    protected RabbitMqConsumer(RabbitMqConnection rabbitMqConnection,
        ILogger<RabbitMqConsumer> logger)
    {
        RabbitMqConnection = rabbitMqConnection ?? throw new ArgumentNullException(nameof(rabbitMqConnection));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _channel = RabbitMqConnection.Connection.CreateModel();
        RabbitMqOptions = rabbitMqConnection.Options;
    }

    protected RabbitMqBroker RabbitMqBroker
    {
        get
        {
            if (_rabbitMqBroker == null)
                lock (Lock)
                {
                    if (_rabbitMqBroker == null) _rabbitMqBroker = GetRabbitMqBroker();
                }

            return _rabbitMqBroker;
        }
    }

    public void Dispose()
    {
        RabbitMqConnection.Dispose();
    }

    protected abstract RabbitMqBroker GetRabbitMqBroker();

    protected abstract Task<bool> BasicConsumerReceived(string exchange, string routingKey, byte[] body,
        RabbitMqQueue queue);

    public virtual void BasicConsumer(params string[] routeKeys)
    {
        if ((routeKeys?.Any() ?? false) == false)
            throw new ArgumentNullException(nameof(routeKeys));


        var queues = RabbitMqBroker.Declare(_channel, routeKeys)?.ToArray();

        if (queues?.Any() ?? false)
        {
            foreach (var queue in queues)
                Logger.LogDebug(
                    $"RabbitMq:[{RabbitMqOptions.Name}] queueName {queue.Name},routeKey:{queue.RoutingKey} bind succeeded.");

            var consumer = new EventingBasicConsumer(_channel);
            foreach (var queue in queues)
            {
                _channel.BasicConsume(queue.Name, queue.AutoAck, queue.ConsumerTag, false, queue.Exclusive,
                    queue.Arguments, consumer);
                Logger.LogDebug(
                    $"RabbitMq:[{RabbitMqOptions.Name}] queueName:{queue.Name} consumer succeeded,routeKey:{queue.RoutingKey}");
            }

            var dic = queues.ToDictionary(item => item.RoutingKey, item => item);

            consumer.Received += async (model, ea) =>
            {
                Logger.LogDebug($"RabbitMq:[{RabbitMqOptions.Name}] message received,routeKey:{ea.RoutingKey}");
                var queue = dic[ea.RoutingKey];
                var result = await BasicConsumerReceived(ea.Exchange, ea.RoutingKey, ea.Body.ToArray(), queue);
                if (queue.AutoAck) return;
                if (result)
                    _channel.BasicAck(ea.DeliveryTag, queue.AckMultiple);
                else
                    _channel.BasicReject(ea.DeliveryTag, queue.RejectRequeue);
            };
        }
        Logger.LogWarning($"RabbitMq:[{RabbitMqOptions.Name}] no queue bind,routeKeys:{string.Join(",", routeKeys)}");
    }
}