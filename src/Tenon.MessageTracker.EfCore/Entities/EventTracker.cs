using Tenon.Repository.EfCore;

namespace Tenon.MessageTracker.EfCore.Entities;

public sealed class EventTracker : EfBasicAuditEntity
{
    public long EventId { get; set; }

    public string TrackerName { get; set; } = string.Empty;
}