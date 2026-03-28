using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Collections;

namespace Tenon.Repository.EfCore.Interceptors;

/// <summary>
/// 完整审计字段拦截器，用于自动设置实体的创建、更新和删除相关字段。
/// </summary>
public class FullAuditableFieldsInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUser<long> userResolver;

    /// <summary>
    /// 初始化 <see cref="FullAuditableFieldsInterceptor"/> 类的新实例。
    /// </summary>
    /// <param name="currentUser">当前用户上下文，用于获取当前用户ID</param>
    public FullAuditableFieldsInterceptor(ICurrentUser<long> currentUser)
    {
        this.userResolver = currentUser;
    }

    /// <summary>
    /// 在保存更改前处理审计字段。
    /// </summary>
    /// <param name="eventData">上下文事件数据</param>
    /// <param name="result">拦截结果</param>
    /// <returns>拦截结果</returns>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context != null)
        {
            // 处理用户审计字段
            HandleUserAuditFields(eventData.Context);
        }
        
        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// 在异步保存更改前处理审计字段。
    /// </summary>
    /// <param name="eventData">上下文事件数据</param>
    /// <param name="result">拦截结果</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>拦截结果</returns>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context != null)
        {
            // 处理用户审计字段
            HandleUserAuditFields(eventData.Context);
        }
        
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// 处理用户审计字段，包括创建者和更新者。
    /// </summary>
    /// <param name="context">数据库上下文</param>
    private void HandleUserAuditFields(DbContext context)
    {
        if (context == null) return;

        var userId = userResolver.UserId;
        if (userId <= 0) return;

        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity<long>>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = userId;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedBy = userId;
                    break;
            }
        }
    }
}