namespace Tenon.Repository.EfCore.SqliteTests.Entities;

public class Blog : EfFullAuditableEntity, IConcurrency, ISoftDelete
{
    public string Url { get; set; }
    public int Rating { get; set; }
    public virtual ICollection<Post> Posts { get; set; } = default!;

    //[ConcurrencyCheck]
    public byte[] RowVersion { get; set; } = default!;
    public bool IsDeleted { get; set; }
}