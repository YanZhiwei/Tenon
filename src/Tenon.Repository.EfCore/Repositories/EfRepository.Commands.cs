using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tenon.Extensions.Collection;
using Tenon.Extensions.Expression;

namespace Tenon.Repository.EfCore;

public partial class EfRepository<TEntity>
    where TEntity : EfEntity, new()
{
    /// <summary>
    /// 异步插入单个实体
    /// </summary>
    public virtual async Task<int> InsertAsync(TEntity entity, CancellationToken token = default)
    {
        await DbContext.Set<TEntity>().AddAsync(entity, token).ConfigureAwait(false);
        return await DbContext.SaveChangesAsync(token).ConfigureAwait(false);
    }

    /// <summary>
    /// 异步插入多个实体
    /// </summary>
    public virtual async Task<int> InsertAsync(IEnumerable<TEntity> entities, CancellationToken token = default)
    {
        await DbContext.Set<TEntity>().AddRangeAsync(entities, token).ConfigureAwait(false);
        return await DbContext.SaveChangesAsync(token).ConfigureAwait(false);
    }

    /// <summary>
    /// 异步更新单个实体
    /// </summary>
    public virtual async Task<int> UpdateAsync(TEntity entity, CancellationToken token = default)
    {
        var entry = DbContext.Entry(entity);
        if (entry.State == EntityState.Detached)
            throw new InvalidOperationException("Entity is not tracked, need to specify updated columns");

        if (entry.State is EntityState.Added or EntityState.Deleted)
            throw new InvalidOperationException($"{nameof(entity)}, The entity state is {nameof(entry.State)}");

        return await DbContext.SaveChangesAsync(token).ConfigureAwait(false);
    }

    /// <summary>
    /// 异步更新多个实体
    /// </summary>
    public virtual async Task<int> UpdateAsync(IEnumerable<TEntity> entities, CancellationToken token = default)
    {
        foreach (var entity in entities)
        {
            var entry = DbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
                throw new InvalidOperationException("Entity is not tracked, need to specify updated columns");

            if (entry.State is EntityState.Added or EntityState.Deleted)
                throw new InvalidOperationException($"{nameof(entity)},The entity state is {nameof(entry.State)}");
        }

        return await DbContext.SaveChangesAsync(token);
    }

    /// <summary>
    /// 异步删除单个实体
    /// </summary>
    public virtual async Task<int> RemoveAsync(TEntity entity, CancellationToken token = default)
    {
        DbContext.Remove(entity);
        return await DbContext.SaveChangesAsync(token);
    }

    /// <summary>
    /// 异步根据主键删除实体
    /// </summary>
    public virtual async Task<int> RemoveAsync(long keyValue, CancellationToken token = default)
    {
        try
        {
            _logger.LogDebug("开始根据主键删除实体，主键值: {KeyValue}", keyValue);
            var entity = await DbContext.Set<TEntity>().AsNoTracking()
                             .FirstOrDefaultAsync(t => t.Id.Equals(keyValue), token).ConfigureAwait(false) ??
                         new TEntity { Id = keyValue };
            DbContext.Remove(entity);
            var result = await DbContext.SaveChangesAsync(token).ConfigureAwait(false);
            _logger.LogDebug("成功删除实体，主键值: {KeyValue}", keyValue);
            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "删除实体时发生并发错误，主键值: {KeyValue}", keyValue);
            return 0;
        }
    }

    /// <summary>
    /// 异步删除多个实体
    /// </summary>
    public virtual async Task<int> RemoveAsync(IEnumerable<TEntity> entities, CancellationToken token = default)
    {
        DbContext.RemoveRange(entities);
        return await DbContext.SaveChangesAsync(token);
    }

    /// <summary>
    /// 异步更新实体的指定属性。支持 Detached 状态实体的局部列更新。
    /// </summary>
    public virtual async Task<int> UpdateAsync(TEntity entity, Expression<Func<TEntity, object>>[] updatingExpressions,
        CancellationToken token = default)
    {
        if (updatingExpressions.IsNullOrEmpty())
            return await UpdateAsync(entity, token);

        var entry = DbContext.Entry(entity);

        if (entry.State is EntityState.Added or EntityState.Deleted)
            throw new InvalidOperationException($"{nameof(entity)} state is {entry.State}, cannot perform partial update");

        if (entry.State == EntityState.Detached)
        {
            entry.State = EntityState.Unchanged;
            foreach (var expression in updatingExpressions)
                entry.Property(expression).IsModified = true;
        }
        else
        {
            var propNames = updatingExpressions.Select(x => x.GetMemberName()).ToArray();
            foreach (var propEntry in entry.Properties)
                propEntry.IsModified = propNames.Contains(propEntry.Metadata.Name);
        }

        return await DbContext.SaveChangesAsync(token);
    }
}
