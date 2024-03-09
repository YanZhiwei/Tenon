using System.Threading.Tasks;

namespace Tenon.MessageTracker.Abstractions
{
    public interface IMessageTracker
    {
        string Name { get; }
        Task<bool> HasProcessedAsync(long eventId, string trackerName);
        Task MarkAsProcessedAsync(long eventId, string trackerName);
    }
}