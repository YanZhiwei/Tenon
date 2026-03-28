using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Tenon.Repository.EfCore.Extensions;

namespace Tenon.Repository.EfCore;

/// <summary>
///     Tenon基础数据库上下文
/// </summary>
public abstract class TenonDbContext : DbContext
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="options">数据库上下文选项</param>
    protected TenonDbContext(DbContextOptions options) : base(options)
    {
    }

    /// <summary>
    ///     获取实体所在程序集
    /// </summary>
    protected abstract Assembly EntityAssembly { get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(EntityAssembly);
        modelBuilder.ApplySoftDeleteQueryFilter();
    }
}