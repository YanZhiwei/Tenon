using DotNetCore.CAP;
using Tenon.EventBus.Models;

namespace Tenon.EventBus.Cap;

public sealed class CapEventBusPublisher(ICapPublisher eventBus) : IEventBusPublisher
{
    private readonly ICapPublisher _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));

    public async Task PublishAsync<T>(T evenDescriptor, CancellationToken cancellationToken = default)
        where T : EventBusDescriptor
    {
        await _eventBus.PublishAsync(typeof(T).Name, evenDescriptor, string.Empty, cancellationToken);
    }

    public void Publish<T>(T evenDescriptor) where T : EventBusDescriptor
    {
        _eventBus.Publish(typeof(T).Name, evenDescriptor, string.Empty);
    }
}