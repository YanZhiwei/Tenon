using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Collections;

namespace Tenon.Repository.EfCore.Interceptors;

/// <summary>
/// 删除审计字段拦截器，用于处理软删除实体的相关字段。
/// </summary>
public class DeletionAuditableFieldsInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUser<long> userResolver;

    /// <summary>
    /// 初始化 <see cref="DeletionAuditableFieldsInterceptor"/> 类的新实例。
    /// </summary>
    /// <param name="currentUser">当前用户上下文，用于获取当前用户ID</param>
    public DeletionAuditableFieldsInterceptor(ICurrentUser<long> currentUser)
    {
        this.userResolver = currentUser;
    }

    /// <summary>
    /// 在保存更改前处理软删除相关字段。
    /// </summary>
    /// <param name="eventData">上下文事件数据</param>
    /// <param name="result">拦截结果</param>
    /// <returns>拦截结果</returns>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context != null)
        {
            // 处理软删除字段
            HandleSoftDelete(eventData.Context);
        }
        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// 在异步保存更改前处理软删除相关字段。
    /// </summary>
    /// <param name="eventData">上下文事件数据</param>
    /// <param name="result">拦截结果</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>拦截结果</returns>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context != null)
        {
            // 处理软删除字段
            HandleSoftDelete(eventData.Context);
        }
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// 处理软删除实体的相关字段。
    /// </summary>
    /// <param name="context">数据库上下文</param>
    private void HandleSoftDelete(DbContext context)
    {
        if (context == null) return;

        var userId = userResolver.UserId;
        if (userId <= 0) return;

        var now = DateTimeOffset.UtcNow;
        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);

        var entries = context.ChangeTracker.Entries<IDeletionAuditable<long>>()
            .Where(e => e.State != EntityState.Deleted && e.Entity.IsDeleted)
            .ToList();

        foreach (var entry in entries)
            ApplySoftDelete(entry.Entity, entry.Metadata.GetNavigations()
                .Where(n => typeof(IDeletionAuditable<long>).IsAssignableFrom(n.TargetEntityType.ClrType))
                .ToList(), entry, userId, now, visited);
    }

    private static void ApplySoftDelete(
        IDeletionAuditable<long> entity,
        IReadOnlyList<Microsoft.EntityFrameworkCore.Metadata.INavigation> navigations,
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry,
        long userId,
        DateTimeOffset now,
        HashSet<object> visited)
    {
        if (!visited.Add(entity)) return;

        if (entity.IsDeleted && !entity.DeletedAt.HasValue)
        {
            entity.DeletedAt = now;
            entity.DeletedBy = userId;
        }

        foreach (var navigation in navigations)
        {
            var navigationValue = entry.Navigation(navigation).CurrentValue;
            if (navigationValue == null) continue;

            if (navigation.IsCollection)
            {
                foreach (var item in ((IEnumerable)navigationValue).Cast<IDeletionAuditable<long>>())
                {
                    if (visited.Contains(item)) continue;
                    if (!item.IsDeleted)
                    {
                        item.IsDeleted = true;
                        item.DeletedAt = now;
                        item.DeletedBy = userId;
                    }
                    visited.Add(item);
                }
            }
            else
            {
                var item = (IDeletionAuditable<long>)navigationValue;
                if (visited.Contains(item)) continue;
                if (!item.IsDeleted)
                {
                    item.IsDeleted = true;
                    item.DeletedAt = now;
                    item.DeletedBy = userId;
                }
                visited.Add(item);
            }
        }
    }
}
