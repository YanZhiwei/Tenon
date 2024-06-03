namespace Tenon.Repository.EfCore;

public class EfBasicAuditEntity : EfEntity, IBasicAuditable
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}