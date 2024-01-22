using System.Threading;
using System.Threading.Tasks;
using Tenon.EventBus.Models;

namespace Tenon.EventBus
{
    public interface IEventBusPublisher
    {
        Task PublishAsync<T>(T evenDescriptor, CancellationToken cancellationToken = default)
            where T : EventBusDescriptor;

        void Publish<T>(T evenDescriptor) where T : EventBusDescriptor;
    }
}