namespace Tenon.Repository.EfCore;

public class EfFullAuditableEntity : EfEntity, IFullAuditable<long>
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public long CreatedBy { get; set; }
    public long UpdatedBy { get; set; }
    public long DeletedBy { get; set; }
}