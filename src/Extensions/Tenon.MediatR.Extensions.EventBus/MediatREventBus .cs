using MediatR;

namespace Tenon.MediatR.Extensions.EventBus;

public class MediatREventBus(IMediator mediator) : IEventBus
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
    {
        await _mediator.Publish(@event);
    }
}