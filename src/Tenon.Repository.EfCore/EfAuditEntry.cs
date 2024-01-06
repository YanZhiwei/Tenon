using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Tenon.Repository.EfCore;

[EntityTypeConfiguration(typeof(EfAuditEntryConfiguration))]
public class EfAuditEntry : IAuditEntity
{
    public List<PropertyEntry> TempProperties { get; set; }
    public long Id { get; set; }
    public string EntityName { get; set; }
    public string ActionType { get; set; }
    public long NameIdentifier { get; set; }
    public DateTime TimeStamp { get; set; }
    public string? EntityId { get; set; }
    public Dictionary<string, object?> Changes { get; set; }
}