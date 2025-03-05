using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Tenon.Repository.EfCore.Interceptors;

/// <summary>
/// 多租户拦截器，用于自动设置租户ID
/// </summary>
public class TenantInterceptor : SaveChangesInterceptor
{
    private readonly IEfTenantResolver _tenantResolver;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="tenantResolver">租户解析器</param>
    public TenantInterceptor(IEfTenantResolver tenantResolver)
    {
        _tenantResolver = tenantResolver ?? throw new ArgumentNullException(nameof(tenantResolver));
    }

    /// <summary>
    /// 在保存更改前处理
    /// </summary>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context != null)
        {
            ApplyTenantId(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// 在异步保存更改前处理
    /// </summary>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context != null)
        {
            ApplyTenantId(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// 应用租户ID
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    private void ApplyTenantId(DbContext dbContext)
    {
        var tenantId = _tenantResolver.TenantId;
        var entries = dbContext.ChangeTracker.Entries<ITenant<long>>().Where(e => 
            e.State == EntityState.Added);

        foreach (var entry in entries)
        {
            entry.Entity.TenantId = tenantId;
        }
    }
}
