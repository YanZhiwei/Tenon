namespace Tenon.Repository.EfCore;

/// <summary>
/// 完整审计实体基类，实现了完整的审计和软删除功能。
/// </summary>
public class EfFullAuditableEntity : EfEntity, IFullAuditable<long>, IDeletionAuditable<long>
{
    /// <summary>
    /// 获取或设置创建时间。
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// 获取或设置更新时间。
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// 获取或设置创建者用户标识。
    /// </summary>
    public long CreatedBy { get; set; }

    /// <summary>
    /// 获取或设置更新者用户标识。
    /// </summary>
    public long? UpdatedBy { get; set; }

    /// <summary>
    /// 获取或设置删除者用户标识。
    /// </summary>
    public long? DeletedBy { get; set; }

    /// <summary>
    /// 获取或设置删除时间。
    /// </summary>
    public DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    /// 获取或设置实体是否已被删除。
    /// </summary>
    public bool IsDeleted { get; set; }
}