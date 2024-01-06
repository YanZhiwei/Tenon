using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenon.Repository.EfCore
{
    public class EfBasicAuditEntity : EfEntity, IBasicAuditable<long>
    {
        public long CreateBy { get; set; }
        public DateTime CreateTime { get; set; }
        public long ModifyBy { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}
