using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Tenon.Infra.RabbitMq.Configurations;
using Tenon.Infra.RabbitMq.Models;

namespace Tenon.Infra.RabbitMq;

public abstract class RabbitMqConsumer : IDisposable
{
    private static readonly object Lock = new();
    protected readonly ILogger<RabbitMqConsumer> Logger;
    protected readonly RabbitMqConnection RabbitMqConnection;
    protected readonly RabbitMqOptions RabbitMqOptions;
    private IModel _channel;

    private RabbitMqQueue? _rabbitMqQueue;

    protected RabbitMqConsumer(RabbitMqConnection rabbitMqConnection,
        ILogger<RabbitMqConsumer> logger)
    {
        RabbitMqConnection = rabbitMqConnection ?? throw new ArgumentNullException(nameof(rabbitMqConnection));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        RabbitMqOptions = rabbitMqConnection.Options;
    }

    protected RabbitMqQueue RabbitMqQueue
    {
        get
        {
            if (_rabbitMqQueue == null)
                lock (Lock)
                {
                    if (_rabbitMqQueue == null)
                        _rabbitMqQueue = GetRabbitMqQueue();
                }

            return _rabbitMqQueue;
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }

    protected abstract RabbitMqQueue GetRabbitMqQueue();

    public abstract Task<bool> BasicConsumerReceived(string exchange, string routingKey, byte[] body,
        RabbitMqQueue queue);

    public virtual void BasicConsumer()
    {
        if (RabbitMqQueue == null)
            throw new ArgumentNullException(nameof(RabbitMqQueue));
        if (!RabbitMqConnection.IsConnected)
            RabbitMqConnection.TryConnect();
        _channel = RabbitMqConnection.Connection.CreateModel();
        _channel.QueueDeclare(RabbitMqQueue.Name,
            RabbitMqQueue.Durable,
            RabbitMqQueue.Exclusive,
            RabbitMqQueue.AutoDelete,
            null);
        Logger.LogDebug(
            $"RabbitMq:[{RabbitMqQueue.Name}] queueName: {RabbitMqQueue.Name} declare succeeded.");

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var currentQueue = ((EventingBasicConsumer)model!).Model.CurrentQueue;
            Logger.LogDebug(
                $"RabbitMq:[{RabbitMqOptions.Name}] message received,queue:{currentQueue},routeKey:{ea.RoutingKey}");

            var result = await BasicConsumerReceived(ea.Exchange, ea.RoutingKey, ea.Body.ToArray(), RabbitMqQueue);
            if (RabbitMqQueue.AutoAck) return;
            if (result)
                _channel.BasicAck(ea.DeliveryTag, RabbitMqQueue.AckMultiple);
            else
                _channel.BasicReject(ea.DeliveryTag, RabbitMqQueue.RejectRequeue);
        };

        _channel.BasicConsume(RabbitMqQueue.Name, RabbitMqQueue.AutoAck, RabbitMqQueue.ConsumerTag, false,
            RabbitMqQueue.Exclusive,
            RabbitMqQueue.Arguments, consumer);
        Logger.LogDebug(
            $"RabbitMq:[{RabbitMqOptions.Name}] queueName:{RabbitMqQueue.Name} consumer succeeded");
    }
}