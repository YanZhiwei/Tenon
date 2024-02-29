using System.Text;

namespace Tenon.Infra.RabbitMq.Models;

public sealed class RabbitMqMessage
{
    public readonly byte[] Body;
    public readonly RabbitMqChannel? Channel;
    public readonly bool ChannelDeclare;
    public readonly Encoding? Encoding;
    public readonly string ExchangeName;
    public readonly bool IsDurable;
    public readonly string Message;
    public readonly string RoutingKey;

    public RabbitMqMessage(string message, string routingKey, string? exchangeName = null,
        RabbitMqChannel? channel = null, Encoding? encoding = null,
        bool isDurable = true)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        ExchangeName = (channel?.Exchange?.Name ?? exchangeName) ??
                       throw new ArgumentNullException(nameof(exchangeName));
        RoutingKey = routingKey ?? throw new ArgumentNullException(nameof(routingKey));
        Encoding = encoding ?? Encoding.UTF8;
        IsDurable = isDurable;
        Channel = channel;
        ChannelDeclare = channel != null;
        Body = Encoding.GetBytes(Message);
    }
}