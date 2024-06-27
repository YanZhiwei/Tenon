using MediatR;

namespace MediatREventBusSample;

public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
{
    public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Order Created: {notification.OrderId} at {notification.OccurredOn}");
        // 在这里添加处理逻辑，例如发送邮件、更新数据库等
        return Task.CompletedTask;
    }
}