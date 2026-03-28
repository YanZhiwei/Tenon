namespace Tenon.Repository.EfCore;

/// <summary>
/// 创建与修改审计实体基类，实现了创建和更新时间的跟踪。
/// </summary>
public class EfCreationModificationAuditEntity : EfEntity, ICreationModificationAuditable
{
    /// <summary>
    /// 获取或设置创建时间。
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// 获取或设置更新时间。
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }
}