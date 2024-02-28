using System.Threading;
using System.Threading.Tasks;
using Tenon.EventBus.Abstractions.Models;

namespace Tenon.EventBus.Abstractions
{
    public interface IEventBusPublisher
    {
        Task PublishAsync<T>(T evenDescriptor, CancellationToken cancellationToken = default)
            where T : EventBusDescriptor;

        void Publish<T>(T evenDescriptor) where T : EventBusDescriptor;
    }
}