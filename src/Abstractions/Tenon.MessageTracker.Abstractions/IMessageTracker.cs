using System.Threading;
using System.Threading.Tasks;

namespace Tenon.MessageTracker.Abstractions
{
    public interface IMessageTracker
    {
        string Name { get; }
        Task<bool> HasProcessedAsync(long eventId, string trackerName, CancellationToken token = default);
        Task MarkAsProcessedAsync(long eventId, string trackerName, CancellationToken token = default);
    }
}