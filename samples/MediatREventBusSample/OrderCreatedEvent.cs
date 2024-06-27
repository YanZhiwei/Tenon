using Tenon.MediatR.Extensions.EventBus;

namespace MediatREventBusSample;

public class OrderCreatedEvent : EventBase
{
    public OrderCreatedEvent(int orderId)
    {
        OrderId = orderId;
    }

    public int OrderId { get; }
}