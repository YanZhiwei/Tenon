namespace Tenon.Repository.EfCore;

/// <summary>
/// 完整审计实体基类，实现了完整的审计和软删除功能。
/// 包含创建时间、创建者、更新时间、更新者以及软删除相关信息。
/// </summary>
public class EfFullAuditableEntity : EfEntity, IFullAuditable<long>
{
    /// <summary>
    /// 获取或设置创建时间。
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// 获取或设置创建者ID。
    /// </summary>
    public long CreatedBy { get; set; }

    /// <summary>
    /// 获取或设置更新时间。
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// 获取或设置更新者ID。
    /// </summary>
    public long? UpdatedBy { get; set; }

    /// <summary>
    /// 获取或设置实体是否已被软删除。
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 获取或设置软删除时间。
    /// </summary>
    public DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    /// 获取或设置执行软删除操作的用户ID。
    /// </summary>
    public long? DeletedBy { get; set; }
}