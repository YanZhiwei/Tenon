using Microsoft.EntityFrameworkCore;
using Tenon.Repository.EfCore;
using Tenon.Repository.EfCore.Extensions;

namespace Tenon.Repository.EfCore.MultiTenant;

/// <summary>
/// 多租户数据库上下文基类，继承自 TenonDbContext 并自动应用租户过滤器
/// </summary>
public abstract class TenantDbContext : TenonDbContext
{
    private readonly IEfTenantResolver _tenantResolver;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="options">数据库上下文选项</param>
    /// <param name="tenantResolver">租户解析器</param>
    protected TenantDbContext(DbContextOptions options, IEfTenantResolver tenantResolver)
        : base(options)
    {
        _tenantResolver = tenantResolver ?? throw new ArgumentNullException(nameof(tenantResolver));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // base 调用会先执行 ApplySoftDeleteQueryFilter，为所有 IDeletionAuditable 实体设置 !IsDeleted 过滤器。
        // ApplyTenantFilter 会对同时实现 ITenant 的实体用 "TenantId == x && !IsDeleted" 覆盖前者，
        // 因此租户实体的最终过滤器由 ApplyTenantFilter 统一负责，不存在重复过滤。
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyTenantFilter(_tenantResolver);
    }
}
