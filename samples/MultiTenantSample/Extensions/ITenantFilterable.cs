namespace MultiTenantSample.Extensions;

/// <summary>
/// 租户过滤接口
/// </summary>
/// <typeparam name="TTenantKey">租户ID类型</typeparam>
public interface ITenantFilterable<TTenantKey>
{
    /// <summary>
    /// 禁用租户过滤器
    /// </summary>
    /// <returns>可释放对象，用于恢复租户过滤器</returns>
    IDisposable DisableTenantFilter();
    
    /// <summary>
    /// 切换租户
    /// </summary>
    /// <param name="tenantId">租户ID</param>
    /// <returns>可释放对象，用于恢复原租户</returns>
    IDisposable ChangeTenant(TTenantKey tenantId);
}
