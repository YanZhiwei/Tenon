using Tenon.Repository.EfCore;

namespace Tenon.MessageTracker.EfCore.Entities;

/// <summary>
/// 事件跟踪实体
/// </summary>
public sealed class EventTracker : EfTimestampAuditEntity
{
    public long EventId { get; set; }

    public string TrackerName { get; set; } = string.Empty;
}