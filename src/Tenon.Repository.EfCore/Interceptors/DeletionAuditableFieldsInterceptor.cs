using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Collections;

namespace Tenon.Repository.EfCore.Interceptors;

/// <summary>
/// 删除审计字段拦截器，用于处理软删除实体的相关字段。
/// </summary>
public class DeletionAuditableFieldsInterceptor : SaveChangesInterceptor
{
    private readonly IUserResolver<long> userResolver;

    /// <summary>
    /// 初始化 <see cref="DeletionAuditableFieldsInterceptor"/> 类的新实例。
    /// </summary>
    /// <param name="userResolver">用户解析器，用于获取当前用户ID</param>
    public DeletionAuditableFieldsInterceptor(IUserResolver<long> userResolver)
    {
        this.userResolver = userResolver;
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

        var entries = context.ChangeTracker.Entries<IDeletionAuditable<long>>()
            .Where(e => e.State != EntityState.Deleted && e.Entity.IsDeleted)
            .ToList();

        foreach (var entry in entries)
        {
            // 设置软删除字段
            if (entry.Entity.IsDeleted && !entry.Entity.DeletedAt.HasValue)
            {
                entry.Entity.DeletedAt = DateTimeOffset.UtcNow;
                entry.Entity.DeletedBy = userId;

                // 获取所有导航属性
                var navigations = entry.Metadata.GetNavigations()
                    .Where(n => typeof(IDeletionAuditable<long>).IsAssignableFrom(n.TargetEntityType.ClrType))
                    .ToList();

                foreach (var navigation in navigations)
                {
                    // 获取导航属性的值
                    var navigationValue = entry.Navigation(navigation).CurrentValue;
                    if (navigationValue == null) continue;

                    // 处理集合导航属性
                    if (navigation.IsCollection)
                    {
                        var items = ((IEnumerable)navigationValue).Cast<IDeletionAuditable<long>>();
                        foreach (var item in items)
                        {
                            if (!item.IsDeleted)
                            {
                                item.IsDeleted = true;
                                item.DeletedAt = DateTimeOffset.UtcNow;
                                item.DeletedBy = userId;
                            }
                        }
                    }
                    // 处理单个导航属性
                    else
                    {
                        var item = (IDeletionAuditable<long>)navigationValue;
                        if (!item.IsDeleted)
                        {
                            item.IsDeleted = true;
                            item.DeletedAt = DateTimeOffset.UtcNow;
                            item.DeletedBy = userId;
                        }
                    }
                }
            }
        }
    }
}
