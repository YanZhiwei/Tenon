using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Tenon.Repository.EfCore.Interceptors;

/// <summary>
/// 时间戳审计字段拦截器，用于自动设置实体的创建时间和更新时间。
/// </summary>
public sealed class TimestampAuditableFieldsInterceptor : SaveChangesInterceptor
{
    /// <summary>
    /// 在保存更改前处理时间戳字段。
    /// </summary>
    /// <param name="eventData">上下文事件数据</param>
    /// <param name="result">拦截结果</param>
    /// <returns>拦截结果</returns>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context != null)
        {
            // 处理时间戳字段
            HandleTimestampFields(eventData.Context);
        }
        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// 在异步保存更改前处理时间戳字段。
    /// </summary>
    /// <param name="eventData">上下文事件数据</param>
    /// <param name="result">拦截结果</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>拦截结果</returns>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        if (eventData.Context != null)
        {
            // 处理时间戳字段
            HandleTimestampFields(eventData.Context);
        }
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// 处理时间戳字段，包括创建时间和更新时间。
    /// </summary>
    /// <param name="context">数据库上下文</param>
    private void HandleTimestampFields(DbContext context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries<ICreationModificationAuditable>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTimeOffset.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
                    break;
            }
        }
    }
}