using System;
using System.Collections.Generic;

namespace Tenon.Repository;

public interface IAuditEntity
{
    public long Id { get; set; }
    public string EntityName { get; set; }
    public string ActionType { get; set; }
    public long NameIdentifier { get; set; }
    public DateTime TimeStamp { get; set; }
    public string? EntityId { get; set; }
    public Dictionary<string, object?> Changes { get; set; }
}