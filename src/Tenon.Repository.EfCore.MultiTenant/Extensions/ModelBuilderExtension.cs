using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Tenon.Repository.EfCore.MultiTenant;

namespace Tenon.Repository.EfCore.Extensions;

public static class ModelBuilderExtension
{
    /// <summary>
    /// 应用多租户查询过滤器（运行时动态读取 TenantId，避免在 OnModelCreating 时快照为常量）。
    /// 对实现了 <see cref="ITenant{TKey}"/> 的实体调用 HasQueryFilter 会替换该实体上已有的任何过滤器，
    /// 包括 ApplySoftDeleteQueryFilter 之前设置的纯 !IsDeleted 过滤器；
    /// 对于同时实现 IDeletionAuditable 的租户实体，此方法会将软删条件合并进租户过滤器中一并处理。
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
