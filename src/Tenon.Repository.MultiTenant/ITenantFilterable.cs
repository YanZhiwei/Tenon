namespace Tenon.Repository;

/// <summary>
/// 定义多租户过滤接口
/// </summary>
/// <typeparam name="TTenantKey">租户主键类型</typeparam>
public interface ITenantFilterable<TTenantKey>
{
    /// <summary>
    /// 获取或设置是否启用租户过滤
    /// </summary>
    bool TenantFilterEnabled { get; set; }
    
    /// <summary>
    /// 获取或设置当前租户标识
    /// </summary>
    TTenantKey CurrentTenantId { get; set; }
    
    /// <summary>
    /// 禁用租户过滤
    /// </summary>
    /// <returns>一个可释放的对象，用于在完成操作后恢复租户过滤</returns>
    IDisposable DisableTenantFilter();
    
    /// <summary>
    /// 启用租户过滤
    /// </summary>
    void EnableTenantFilter();
    
    /// <summary>
    /// 切换到指定租户
    /// </summary>
    /// <param name="tenantId">租户标识</param>
    /// <returns>一个可释放的对象，用于在完成操作后恢复原租户</returns>
    IDisposable ChangeTenant(TTenantKey tenantId);
}
