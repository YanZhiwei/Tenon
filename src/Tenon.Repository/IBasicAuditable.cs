using System;

namespace Tenon.Repository
{
    public interface IBasicAuditable<TKey> 
    {
        TKey CreateBy { get; set; }
        DateTime CreateTime { get; set; }

        TKey? ModifyBy { get; set; }

        DateTime? ModifyTime { get; set; }
    }
}
