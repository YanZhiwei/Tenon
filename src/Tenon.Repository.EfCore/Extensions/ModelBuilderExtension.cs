using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Tenon.Repository;

namespace Tenon.Repository.EfCore.Extensions;

/// <summary>
/// ModelBuilder扩展方法
/// </summary>
public static class ModelBuilderExtension
{
    /// <summary>
    /// 应用软删除查询过滤器
    /// </summary>
    public static void ApplySoftDeleteQueryFilter(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            if (typeof(IDeletionAuditable<long>).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(IDeletionAuditable<long>.IsDeleted));
                var falseConstant = Expression.Constant(false);
                var lambda = Expression.Lambda(Expression.Equal(property, falseConstant), parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
    }
    
    /// <summary>
    /// 应用多租户查询过滤器（运行时动态读取 TenantId，避免在 OnModelCreating 时快照为常量）
    /// </summary>
    /// <param name="modelBuilder">模型构建器</param>
    /// <param name="tenantResolver">租户解析器，每次查询执行时从中实时读取 TenantId</param>
    public static void ApplyTenantFilter(this ModelBuilder modelBuilder, IEfTenantResolver tenantResolver)
    {
        var resolverConstant = Expression.Constant(tenantResolver, typeof(IEfTenantResolver));
        var tenantIdAccessor = Expression.Property(resolverConstant, nameof(ITenantResolver<long, long>.TenantId));

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ITenant<long>).IsAssignableFrom(entityType.ClrType))
                continue;

            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var tenantProperty = Expression.Property(parameter, nameof(ITenant<long>.TenantId));
            var tenantEqual = Expression.Equal(tenantProperty, tenantIdAccessor);

            Expression filterBody;
            if (typeof(IDeletionAuditable<long>).IsAssignableFrom(entityType.ClrType))
            {
                var isDeletedProperty = Expression.Property(parameter, nameof(IDeletionAuditable<long>.IsDeleted));
                var notDeleted = Expression.Equal(isDeletedProperty, Expression.Constant(false));
                filterBody = Expression.AndAlso(tenantEqual, notDeleted);
            }
            else
            {
                filterBody = tenantEqual;
            }

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(Expression.Lambda(filterBody, parameter));
        }
    }
}