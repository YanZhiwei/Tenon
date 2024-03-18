using RabbitMQ.Client;
using Tenon.Infra.RabbitMq.Models;

namespace Tenon.Infra.RabbitMq.Extensions;

public static class RabbitMqBrokerExtension
{
    public static IEnumerable<RabbitMqQueue>? Declare(this RabbitMqBroker broker, IModel channel, string[] routeKeys)
    {
        if (channel == null)
            throw new ArgumentNullException(nameof(channel));
        if (!(routeKeys?.Any() ?? false))
            throw new ArgumentNullException(nameof(routeKeys));

        if (broker == null)
            throw new ArgumentNullException(nameof(broker));

        var exchange = broker.Exchange;

        var brokerQueues = broker.Queues ?? null;

        channel.ExchangeDeclare(exchange.Name, exchange.Type.ToString().ToLowerInvariant(),
            exchange.Durable, exchange.AutoDelete);

        if (brokerQueues?.Any() ?? false)
        {
            var queues = brokerQueues.Where(c => routeKeys.Contains(c.RoutingKey, StringComparer.OrdinalIgnoreCase))
                .ToList();
            foreach (var rabbitMqQueue in queues)
            {
                channel.QueueDeclare(rabbitMqQueue.Name,
                    rabbitMqQueue.Durable,
                    rabbitMqQueue.Exclusive,
                    rabbitMqQueue.AutoDelete,
                    rabbitMqQueue.Arguments);
                channel.QueueBind(rabbitMqQueue.Name, rabbitMqQueue.ExchangeName, rabbitMqQueue.RoutingKey);
            }

            return queues;
        }

        return null;
    }
}