using System.Text;

namespace Tenon.Infra.RabbitMq.Models;

public class RabbitMqQueue
{
    public readonly IDictionary<string, object> Arguments;

    public readonly bool AutoAck;
    public readonly bool AutoDelete;

    public readonly string ConsumerTag;

    public readonly bool Durable;

    public readonly string ExchangeName;
    public readonly bool Exclusive;

    public readonly string Name;

    public readonly string RoutingKey;

    public readonly bool RejectRequeue;


    public bool AckMultiple { get; set; }
    public RabbitMqQueue(string name, string exchangeName, string routingKey, bool rejectRequeue = false, bool ackMultiple = false, bool autoAck = true, bool durable = true,
        bool autoDelete = false, bool exclusive = false,
        IDictionary<string, object> arguments = null, string consumerTag = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ExchangeName = exchangeName ?? throw new ArgumentNullException(nameof(exchangeName));
        RoutingKey = routingKey ?? throw new ArgumentNullException(nameof(routingKey));
        RejectRequeue = rejectRequeue;
        AckMultiple = ackMultiple;
        Arguments = arguments;
        ConsumerTag = consumerTag;
        AutoAck = autoAck;
        Durable = durable;
        AutoDelete = autoDelete;
        Exclusive = exclusive;
    }
}