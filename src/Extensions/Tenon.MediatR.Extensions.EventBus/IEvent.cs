using MediatR;

namespace Tenon.MediatR.Extensions.EventBus;

public interface IEvent : INotification
{
}

public abstract class EventBase : IEvent
{
    public DateTimeOffset OccurredOn { get; private set; } = DateTimeOffset.UtcNow;
}