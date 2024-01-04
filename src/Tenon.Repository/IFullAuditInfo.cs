using System;

namespace Tenon.Repository;

public interface IFullAuditInfo<TKey> : IBasicAuditInfo<TKey>
{
    TKey? ModifyBy { get; set; }

    DateTime? ModifyTime { get; set; }
}