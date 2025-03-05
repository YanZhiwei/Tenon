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
    /// 应用多租户查询过滤器
    /// </summary>
    /// <param name="modelBuilder">模型构建器</param>
    /// <param name="tenantId">当前租户ID</param>
    public static void ApplyTenantFilter(this ModelBuilder modelBuilder, long tenantId)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // 检查实体是否实现了ITenant<long>接口
            if (typeof(ITenant<long>).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ITenant<long>.TenantId));
                var tenantIdConstant = Expression.Constant(tenantId);
                var equalExpression = Expression.Equal(property, tenantIdConstant);
                
                // 如果实体同时实现了软删除接口，则组合两个过滤条件
                if (typeof(IDeletionAuditable<long>).IsAssignableFrom(entityType.ClrType))
                {
                    var isDeletedProperty = Expression.Property(parameter, nameof(IDeletionAuditable<long>.IsDeleted));
                    var falseConstant = Expression.Constant(false);
                    var notDeletedExpression = Expression.Equal(isDeletedProperty, falseConstant);
                    
                    // 组合租户过滤和软删除过滤
                    var combinedExpression = Expression.AndAlso(equalExpression, notDeletedExpression);
                    var lambda = Expression.Lambda(combinedExpression, parameter);
                    
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
                else
                {
                    // 只应用租户过滤
                    var lambda = Expression.Lambda(equalExpression, parameter);
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }
    }
}