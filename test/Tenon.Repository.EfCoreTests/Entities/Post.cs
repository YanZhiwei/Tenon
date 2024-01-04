using Tenon.Repository.EfCore;

namespace Tenon.Repository.EfCoreTests.Entities;

public class Post : EfEntity, IFullAuditInfo<long>
{
    public string Title { get; set; }
    public string Content { get; set; }
    public virtual Blog Blog { get; set; }
    public long CreateBy { get; set; }
    public DateTime CreateTime { get; set; }
    public long ModifyBy { get; set; }
    public DateTime? ModifyTime { get; set; }
}