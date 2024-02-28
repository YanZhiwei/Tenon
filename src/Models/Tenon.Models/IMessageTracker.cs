using System.Threading.Tasks;

namespace Tenon.Models
{
    public interface IMessageTracker
    {
        Task<bool> HasProcessedAsync(long eventId, string trackerName);
        Task MarkAsProcessedAsync(long eventId, string trackerName);
    }
}