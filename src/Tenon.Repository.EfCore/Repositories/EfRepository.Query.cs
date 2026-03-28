using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Tenon.Repository.EfCore;

public partial class EfRepository<TEntity>
    where TEntity : EfEntity, new()
{
    /// <summary>
    /// 根据条件查询
    /// </summary>
    /// <param name="expression">查询条件表达式</param>
    /// <param name="noTracking">是否不追踪实体</param>
    public virtual IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression, bool noTracking = true)
    {
        return GetDbSet(noTracking).Where(expression);
    }

    /// <summary>
    /// 异步获取列表
    /// </summary>
    /// <param name="whereExpression">查询条件表达式</param>
    /// <param name="navigationPropertyPath">包含的导航属性</param>
    /// <param name="noTracking">是否不追踪实体</param>
    /// <param name="token">取消令牌</param>
    public virtual async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> whereExpression,
        Expression<Func<TEntity, dynamic>>? navigationPropertyPath = null,
        bool noTracking = true, CancellationToken token = default)
    {
        var query = whereExpression != null ? GetDbSet(noTracking).Where(whereExpression) : GetDbSet(noTracking);
        if (navigationPropertyPath != null) query = query.Include(navigationPropertyPath);
        return await query.ToListAsync(token).ConfigureAwait(false);
    }

    /// <summary>
    /// 异步获取符合条件的列表，并包含指定的导航属性
    /// </summary>
    public async Task<IEnumerable<TEntity>> GetListWithNavigationPropertiesAsync(
        Expression<Func<TEntity, bool>> whereExpression,
        IEnumerable<Expression<Func<TEntity, dynamic>>>? navigationPropertyPaths = null,
        bool noTracking = true, CancellationToken token = default)
    {
        var query = whereExpression != null ? GetDbSet(noTracking).Where(whereExpression) : GetDbSet(noTracking);
        if (navigationPropertyPaths != null)
            foreach (var navigationPath in navigationPropertyPaths)
                query = query.Include(navigationPath);
        return await query.ToListAsync(token).ConfigureAwait(false);
    }

    /// <summary>
    /// 异步根据主键获取实体
    /// </summary>
    public virtual async Task<TEntity?> GetAsync(long keyValue, bool noTracking = true,
        CancellationToken token = default)
    {
        var query = GetDbSet(noTracking).Where(t => t.Id.Equals(keyValue));
        return await query.FirstOrDefaultAsync(token).ConfigureAwait(false);
    }

    /// <summary>
    /// 异步根据主键和导航属性获取实体
    /// </summary>
    public async Task<TEntity?> GetWithNavigationPropertiesAsync(long keyValue,
        IEnumerable<Expression<Func<TEntity, dynamic>>>? navigationPropertyPaths = null, bool noTracking = true,
        CancellationToken token = default)
    {
        var query = GetDbSet(noTracking).Where(t => t.Id == keyValue);
        if (navigationPropertyPaths != null)
            foreach (var navigationPath in navigationPropertyPaths)
                query = query.Include(navigationPath);
        return await query.FirstOrDefaultAsync(token).ConfigureAwait(false);
    }

    /// <summary>
    /// 根据主键和单个导航属性获取实体
    /// </summary>
    public virtual async Task<TEntity?> GetAsync(long keyValue,
        Expression<Func<TEntity, dynamic>>? navigationPropertyPath = null,
        bool noTracking = true, CancellationToken token = default)
    {
        if (navigationPropertyPath != null)
            return await GetWithNavigationPropertiesAsync(keyValue, [navigationPropertyPath], noTracking, token)
                .ConfigureAwait(false);
        return await GetAsync(keyValue, noTracking, token).ConfigureAwait(false);
    }

    /// <summary>
    /// 获取所有实体
    /// </summary>
    public virtual IQueryable<TEntity> GetAll(bool noTracking = true)
    {
        return GetDbSet(noTracking);
    }

    /// <summary>
    /// 异步获取分页列表
    /// </summary>
    public async Task<PagedResult<TEntity>> GetPagedListAsync(
        int pageIndex, int pageSize,
        Expression<Func<TEntity, bool>>? whereExpression = null,
        IEnumerable<Expression<Func<TEntity, dynamic>>>? includeProperties = null,
        bool noTracking = true, CancellationToken token = default)
    {
        var query = whereExpression != null ? GetDbSet(noTracking).Where(whereExpression) : GetDbSet(noTracking);

        if (includeProperties != null)
            foreach (var includeProperty in includeProperties)
                query = query.Include(includeProperty);

        var totalCount = await query.CountAsync(token).ConfigureAwait(false);
        var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(token)
            .ConfigureAwait(false);

        return new PagedResult<TEntity>
        {
            TotalCount = totalCount,
            Items = items,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// 异步检查是否存在满足条件的实体
    /// </summary>
    public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> whereExpression,
        CancellationToken token = default)
    {
        _logger.LogDebug("开始检查是否存在满足条件的实体，查询条件: {WhereExpression}", whereExpression);
        var exists = await DbContext.Set<TEntity>().AsNoTracking().AnyAsync(whereExpression, token);
        _logger.LogDebug("检查结果: {Exists}", exists);
        return exists;
    }

    /// <summary>
    /// 异步获取满足条件的实体数量
    /// </summary>
    public virtual async Task<long> CountAsync(Expression<Func<TEntity, bool>> whereExpression,
        CancellationToken token = default)
    {
        _logger.LogDebug("开始获取满足条件的实体数量，查询条件: {WhereExpression}", whereExpression);
        var count = await DbContext.Set<TEntity>().AsNoTracking().LongCountAsync(whereExpression, token);
        _logger.LogDebug("满足条件的实体数量: {Count}", count);
        return count;
    }

    /// <summary>
    /// 异步获取所有实体
    /// </summary>
    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken token = default)
    {
        return await GetAll().ToListAsync(token);
    }

    /// <summary>
    /// 异步获取满足条件的实体列表
    /// </summary>
    public virtual async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> whereExpression,
        CancellationToken token = default)
    {
        var result = await GetListAsync(whereExpression, null, true, token);
        return result;
    }

    /// <summary>
    /// 异步根据主键获取实体
    /// </summary>
    public virtual async Task<TEntity?> GetAsync(long keyValue, CancellationToken token = default)
    {
        _logger.LogDebug("开始根据主键获取实体，主键值: {KeyValue}", keyValue);
        var entity = await GetAsync(keyValue, true, token);
        _logger.LogDebug("成功获取实体: {Entity}", entity);
        return entity;
    }

    /// <summary>
    /// 异步根据主键和导航属性路径获取实体
    /// </summary>
    public virtual async Task<TEntity?> GetAsync(long keyValue,
        IEnumerable<Expression<Func<TEntity, dynamic>>>? navigationPropertyPaths = null,
        CancellationToken token = default)
    {
        return await GetWithNavigationPropertiesAsync(keyValue, navigationPropertyPaths, true, token);
    }

    /// <summary>
    /// 异步根据主键和单个导航属性路径获取实体
    /// </summary>
    public virtual async Task<TEntity?> GetByKeyWithNavigationAsync(long keyValue,
        Expression<Func<TEntity, dynamic>>? navigationPropertyPath = null, CancellationToken token = default)
    {
        return await GetAsync(keyValue, navigationPropertyPath, true, token);
    }
}
