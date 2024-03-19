using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Tenon.Infra.RabbitMq.Configurations;
using Tenon.Infra.RabbitMq.Models;

namespace Tenon.Infra.RabbitMq;

public sealed class RabbitMqBroker(
    RabbitMqConnection rabbitMqConnection,
    RabbitMqExchange rabbitMqExchange,
    IEnumerable<RabbitMqQueue> rabbitMqQueues,
    ILogger logger)
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IConnection _rabbitMqConnection = rabbitMqConnection != null ? rabbitMqConnection.Connection ?? throw new ArgumentNullException(nameof(rabbitMqConnection)) : throw new ArgumentNullException(nameof(rabbitMqConnection));
    private readonly RabbitMqOptions _rabbitMqOptions = rabbitMqConnection.Options;
    public readonly RabbitMqExchange Exchange = rabbitMqExchange ?? throw new ArgumentNullException(nameof(rabbitMqExchange));
    public readonly string ExchangeName = rabbitMqExchange.Name;
    public readonly IEnumerable<RabbitMqQueue> Queues = rabbitMqQueues ?? throw new ArgumentNullException(nameof(rabbitMqQueues));

    public void Declare()
    {
        using (var channel = _rabbitMqConnection.CreateModel())
        {
            channel.ExchangeDeclare(Exchange.Name, Exchange.Type.ToString().ToLowerInvariant(),
                Exchange.Durable, Exchange.AutoDelete);
            _logger.LogDebug(
                $"RabbitMq:[{_rabbitMqOptions.Name}] exchangeName:{Exchange.Name} type:{Exchange.Type} declare succeeded.");
            if (Queues.Any())
            {
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
            else
            {
                _logger.LogWarning($"RabbitMq:[{_rabbitMqOptions.Name}] exchangeName:{Exchange.Name} queues is null.");
            }
        }
    }
}