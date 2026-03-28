namespace Tenon.Repository;

/// <summary>
/// 删除审计接口，提供软删除功能以及删除审计信息。
/// 实现此接口的实体将支持软删除功能，并记录删除时间和删除者信息。
/// </summary>
/// <typeparam name="TKey">删除者ID的类型</typeparam>
public interface IDeletionAuditable<TKey> where TKey : struct
{
    /// <summary>
    /// 获取或设置一个值，指示实体是否已被软删除。
    /// 默认值为 false，表示实体未被删除。
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// 获取或设置实体被软删除的时间。
    /// 如果实体未被删除，则此值为 null。
    /// </summary>
    DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    /// 获取或设置执行软删除操作的用户ID。
    /// 如果实体未被删除，则此值为默认值。
    /// </summary>
    TKey? DeletedBy { get; set; }
}
