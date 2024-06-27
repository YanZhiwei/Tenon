namespace Tenon.MediatR.Extensions.EventBus;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent;
}