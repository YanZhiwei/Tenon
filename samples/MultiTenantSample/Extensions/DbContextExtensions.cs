using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using MultiTenantSample.Data;
using Tenon.Repository;
using Tenon.Repository.EfCore;

namespace MultiTenantSample.Extensions;

/// <summary>
/// 数据库上下文扩展方法
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// 禁用租户过滤器
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    /// <returns>可释放对象，用于恢复租户过滤器</returns>
    public static IDisposable DisableTenantFilter(this MultiTenantDbContext dbContext)
    {
        var tenantResolver = dbContext.GetService<IServiceProvider>().GetRequiredService<IEfTenantResolver>();
        if (tenantResolver is ITenantFilterable<long> filterableTenant)
        {
            return filterableTenant.DisableTenantFilter();
        }
        
        return new DisposeAction(() => { });
    }
    
    /// <summary>
    /// 切换租户
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    /// <param name="tenantId">租户ID</param>
    /// <returns>可释放对象，用于恢复原租户</returns>
    public static IDisposable ChangeTenant(this MultiTenantDbContext dbContext, long tenantId)
    {
        var tenantResolver = dbContext.GetService<IServiceProvider>().GetRequiredService<IEfTenantResolver>();
        if (tenantResolver is ITenantFilterable<long> filterableTenant)
        {
            return filterableTenant.ChangeTenant(tenantId);
        }
        
        return new DisposeAction(() => { });
    }
}
