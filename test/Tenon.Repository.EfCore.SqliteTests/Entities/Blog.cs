using Microsoft.EntityFrameworkCore;

namespace Tenon.Repository.EfCore.SqliteTests.Entities
{
    [PrimaryKey(nameof(Id))]
    public class Blog : EfBasicAuditEntity, IFullAuditable
    {
        public string Url { get; set; }
        public int Rating { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
