using System.Text;

namespace Tenon.Infra.RabbitMq.Models;

public sealed class RabbitMqMessage
{
    public readonly byte[] Body;
    public readonly Encoding? Encoding;
    public readonly bool Durable;
    public readonly string Message;
    public readonly string RoutingKey;

    public RabbitMqMessage(string message, string routingKey,
        Encoding? encoding = null,
        bool durable = true)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        RoutingKey = routingKey ?? throw new ArgumentNullException(nameof(routingKey));
        Encoding = encoding ?? Encoding.UTF8;
        Durable = durable;
        Body = Encoding.GetBytes(Message);
    }
}