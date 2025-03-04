namespace Tenon.Repository.EfCore;

public class EfBasicAuditEntity : EfEntity, ITimestampAuditable
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}