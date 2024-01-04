using System;

namespace Tenon.Repository
{
    public interface IBasicAuditInfo<TKey>
    {
        TKey CreateBy { get; set; }
        DateTime CreateTime { get; set; }
    }
}
