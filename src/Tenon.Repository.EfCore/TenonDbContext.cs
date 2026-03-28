using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Tenon.Repository.EfCore.Extensions;

namespace Tenon.Repository.EfCore;

/// <summary>
///     Tenon基础数据库上下文
/// </summary>
public abstract class TenonDbContext : DbContext
{
    private readonly IEfTenantResolver _tenantResolver;
    private readonly bool _multiTenancyEnabled;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="options">数据库上下文选项</param>
    protected TenonDbContext(DbContextOptions options) : base(options)
    {
        _multiTenancyEnabled = false;
    }
    
    /// <summary>
    /// 构造函数（支持多租户）
    /// </summary>
    /// <param name="options">数据库上下文选项</param>
    /// <param name="tenantResolver">租户解析器</param>
    protected TenonDbContext(DbContextOptions options, IEfTenantResolver tenantResolver) : base(options)
    {
        _tenantResolver = tenantResolver;
        _multiTenancyEnabled = true;
    }

    /// <summary>
    ///     获取实体所在程序集
    /// </summary>
    protected abstract Assembly EntityAssembly { get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 应用所有实体配置
        modelBuilder.ApplyConfigurationsFromAssembly(EntityAssembly);

        // 为所有实现了IDeletionAuditable的实体添加全局软删除过滤器
        modelBuilder.ApplySoftDeleteQueryFilter();
        
        // 如果启用了多租户，为所有实现了ITenant的实体添加全局租户过滤器
        // 传递 resolver 引用而非快照值，确保每次查询时动态读取当前 TenantId
        if (_multiTenancyEnabled && _tenantResolver != null)
        {
            modelBuilder.ApplyTenantFilter(_tenantResolver);
        }
    }
}