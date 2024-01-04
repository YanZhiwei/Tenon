using Microsoft.EntityFrameworkCore;
using Tenon.Repository.EfCore;

namespace Tenon.Repository.EfCoreTests.Entities
{
    [PrimaryKey(nameof(Id))]
    public class Blog : EfEntity, IBasicAuditInfo<long>
    {
        public string Url { get; set; }
        public int Rating { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public long CreateBy { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
