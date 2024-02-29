namespace Tenon.Infra.RabbitMq.Models;

public class RabbitMqChannel(string name, RabbitMqExchange exchange, IEnumerable<RabbitMqQueue> queues)
{
    public readonly RabbitMqExchange Exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));

    public readonly string Name = name ?? throw new ArgumentNullException(nameof(name));

    public readonly IEnumerable<RabbitMqQueue> Queues = queues ?? throw new ArgumentNullException(nameof(queues));
}