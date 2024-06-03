using Microsoft.EntityFrameworkCore;

namespace Tenon.Repository.EfCore.MySqlTests.Entities;

[PrimaryKey(nameof(Id))]
public class Blog : EfBasicAuditEntity, IConcurrency
{
    public string Url { get; set; }
    public int Rating { get; set; }
    public virtual ICollection<Post> Posts { get; set; } = default!;
    public byte[] RowVersion { get; set; } = default!;
}