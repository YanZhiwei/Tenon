namespace Tenon.Repository;

/// <summary>
/// 完整审计实体接口，聚合创建/修改时间、创建/更新者以及软删除的全量审计能力。
/// 实现此接口的实体将记录创建时间、创建者、更新时间、更新者以及软删除相关信息。
/// </summary>
/// <typeparam name="TKey">用户标识的类型</typeparam>
public interface IAuditableEntity<TKey> : ICreationModificationAuditable, IDeletionAuditable<TKey>
    where TKey : struct
{
    /// <summary>
    /// 获取或设置创建实体的用户标识。
    /// </summary>
    TKey CreatedBy { get; set; }

    /// <summary>
    /// 获取或设置最后更新实体的用户标识。
    /// 如果实体未被更新过，则此值可能与 CreatedBy 相同。
    /// </summary>
    TKey? UpdatedBy { get; set; }
}
