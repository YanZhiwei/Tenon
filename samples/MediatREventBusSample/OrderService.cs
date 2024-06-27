using Tenon.MediatR.Extensions.EventBus;

namespace MediatREventBusSample;

public sealed class OrderService: IOrderService
{
    private readonly IEventBus _eventBus;

    public OrderService(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task CreateOrder(int orderId)
    {
        // 创建订单逻辑
        // ...

        // 发布事件
        var orderCreatedEvent = new OrderCreatedEvent(orderId);
        await _eventBus.PublishAsync(orderCreatedEvent);
    }
}