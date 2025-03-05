namespace Tenon.Repository.EfCore;

/// <summary>
/// 多租户完整审计实体基类
/// </summary>
public abstract class EfTenantFullAuditableEntity : EfFullAuditableEntity, ITenant<long>
{
    /// <summary>
    /// 获取或设置租户标识
    /// </summary>
    public long TenantId { get; set; }
}
