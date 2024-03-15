using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Tenon.Infra.RabbitMq.Configurations;
using Tenon.Infra.RabbitMq.Models;

namespace Tenon.Infra.RabbitMq;

public sealed class RabbitMqBroker
{
    private readonly ILogger<RabbitMqProducer> _logger;
    private readonly IConnection _rabbitMqConnection;
    private readonly RabbitMqOptions _rabbitMqOptions;
    public readonly RabbitMqExchange Exchange;
    public readonly string ExchangeName;
    public readonly IEnumerable<RabbitMqQueue> Queues;

    public RabbitMqBroker(RabbitMqConnection rabbitMqConnection, RabbitMqExchange rabbitMqExchange,
        IEnumerable<RabbitMqQueue> rabbitMqQueues,
        ILogger<RabbitMqProducer> logger)
    {
        _rabbitMqConnection = rabbitMqConnection?.Connection ??
                              throw new ArgumentNullException(nameof(rabbitMqConnection));
        Exchange = rabbitMqExchange ?? throw new ArgumentNullException(nameof(rabbitMqExchange));
        Queues = rabbitMqQueues ?? throw new ArgumentNullException(nameof(rabbitMqQueues));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _rabbitMqOptions = rabbitMqConnection.Options;
        ExchangeName = rabbitMqExchange.Name;
    }

    public void Declare()
    {
        using (var channel = _rabbitMqConnection.CreateModel())
        {
            channel.ExchangeDeclare(Exchange.Name, Exchange.Type.ToString().ToLowerInvariant(),
                Exchange.Durable, Exchange.AutoDelete);
            _logger.LogDebug(
                $"RabbitMq:[{_rabbitMqOptions.Name}] exchangeName:{Exchange.Name} type:{Exchange.Type} declare succeeded.");
            if (Queues?.Any() ?? false)
                foreach (var rabbitMqQueue in Queues)
                {
                    channel.QueueDeclare(rabbitMqQueue.Name,
                        rabbitMqQueue.Durable,
                        rabbitMqQueue.Exclusive,
                        rabbitMqQueue.AutoDelete,
                        null);
                    _logger.LogDebug(
                        $"RabbitMq:[{_rabbitMqOptions.Name}] queueName: {rabbitMqQueue.Name} declare succeeded.");
                    channel.QueueBind(rabbitMqQueue.Name, rabbitMqQueue.ExchangeName, rabbitMqQueue.RoutingKey);
                    _logger.LogDebug(
                        $"RabbitMq:[{_rabbitMqOptions.Name}] queueName {rabbitMqQueue.Name},routeKey:{rabbitMqQueue.RoutingKey} bind succeeded.");
                }
        }
    }

    public IEnumerable<RabbitMqQueue>? Declare(IModel channel, string[] routeKeys)
    {
        if (channel == null)
            throw new ArgumentNullException(nameof(channel));
        if (!(routeKeys?.Any() ?? false))
            throw new ArgumentNullException(nameof(routeKeys));

        channel.ExchangeDeclare(Exchange.Name, Exchange.Type.ToString().ToLowerInvariant(),
            Exchange.Durable, Exchange.AutoDelete);
        _logger.LogDebug(
            $"RabbitMq:[{_rabbitMqOptions.Name}] exchangeName:{Exchange.Name} type:{Exchange.Type} declare succeeded.");
        if (Queues?.Any() ?? false)
        {
            var queues = Queues.Where(c => routeKeys.Contains(c.RoutingKey, StringComparer.OrdinalIgnoreCase)).ToList();
            foreach (var rabbitMqQueue in queues)
            {
                channel.QueueDeclare(rabbitMqQueue.Name,
                    rabbitMqQueue.Durable,
                    rabbitMqQueue.Exclusive,
                    rabbitMqQueue.AutoDelete,
                    rabbitMqQueue.Arguments);
                _logger.LogDebug(
                    $"RabbitMq:[{_rabbitMqOptions.Name}] queueName: {rabbitMqQueue.Name} declare succeeded.");
                channel.QueueBind(rabbitMqQueue.Name, rabbitMqQueue.ExchangeName, rabbitMqQueue.RoutingKey);
                _logger.LogDebug(
                    $"RabbitMq:[{_rabbitMqOptions.Name}] queueName {rabbitMqQueue.Name},routeKey:{rabbitMqQueue.RoutingKey} bind succeeded.");
            }

            return queues;
        }

        return null;
    }

}