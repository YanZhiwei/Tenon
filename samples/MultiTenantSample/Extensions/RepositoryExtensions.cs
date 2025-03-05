using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Tenon.Repository;
using Tenon.Repository.EfCore;

namespace MultiTenantSample.Extensions;

/// <summary>
/// 仓储扩展方法
/// </summary>
public static class RepositoryExtensions
{
    /// <summary>
    /// 禁用租户过滤器
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="repository">租户仓储</param>
    /// <returns>可释放对象，用于恢复租户过滤器</returns>
    public static IDisposable DisableTenantFilter<TEntity>(this ITenantRepository<TEntity, long, long> repository)
        where TEntity : IEntity<long>, ITenant<long>
    {
        if (repository is ITenantFilterable<long> filterableTenant)
        {
            return filterableTenant.DisableTenantFilter();
        }
        
        return new DisposeAction(() => { });
    }
    
    /// <summary>
    /// 切换租户
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="repository">租户仓储</param>
    /// <param name="tenantId">租户ID</param>
    /// <returns>可释放对象，用于恢复原租户</returns>
    public static IDisposable ChangeTenant<TEntity>(this ITenantRepository<TEntity, long, long> repository, long tenantId)
        where TEntity : IEntity<long>, ITenant<long>
    {
        if (repository is ITenantFilterable<long> filterableTenant)
        {
            return filterableTenant.ChangeTenant(tenantId);
        }
        
        return new DisposeAction(() => { });
    }
    
    /// <summary>
    /// 获取当前租户下满足条件的实体列表，并包含导航属性
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="repository">租户仓储</param>
    /// <param name="whereExpression">查询条件表达式</param>
    /// <param name="include">包含导航属性的函数</param>
    /// <param name="token">取消令牌</param>
    /// <returns>实体列表</returns>
    public static async Task<IEnumerable<TEntity>> GetListForCurrentTenantAsync<TEntity>(
        this ITenantRepository<TEntity, long, long> repository,
        Expression<Func<TEntity, bool>> whereExpression = null,
        Func<IQueryable<TEntity>, IQueryable<TEntity>> include = null,
        CancellationToken token = default)
        where TEntity : class, IEntity<long>, ITenant<long>
    {
        // 获取基础查询
        var baseQuery = await repository.GetListForCurrentTenantAsync(whereExpression ?? (e => true), token);
        var query = baseQuery.AsQueryable();
        
        // 应用包含导航属性的函数
        if (include != null)
        {
            query = include(query);
        }
        
        return query.ToList();
    }
    
    /// <summary>
    /// 获取当前租户下满足条件的实体，并包含导航属性
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="repository">租户仓储</param>
    /// <param name="whereExpression">查询条件表达式</param>
    /// <param name="include">包含导航属性的函数</param>
    /// <param name="token">取消令牌</param>
    /// <returns>实体</returns>
    public static async Task<TEntity> GetForCurrentTenantAsync<TEntity>(
        this ITenantRepository<TEntity, long, long> repository,
        Expression<Func<TEntity, bool>> whereExpression,
        Func<IQueryable<TEntity>, IQueryable<TEntity>> include = null,
        CancellationToken token = default)
        where TEntity : class, IEntity<long>, ITenant<long>
    {
        // 获取基础查询
        var entities = await repository.GetListForCurrentTenantAsync(whereExpression, token);
        var query = entities.AsQueryable();
        
        // 应用包含导航属性的函数
        if (include != null)
        {
            query = include(query);
        }
        
        return query.FirstOrDefault();
    }
    
    /// <summary>
    /// 为当前租户插入实体
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="repository">租户仓储</param>
    /// <param name="entity">要插入的实体</param>
    /// <param name="token">取消令牌</param>
    /// <returns>插入后的实体</returns>
    public static async Task<TEntity> InsertForCurrentTenantAsync<TEntity>(
        this ITenantRepository<TEntity, long, long> repository,
        TEntity entity,
        CancellationToken token = default)
        where TEntity : class, IEntity<long>, ITenant<long>
    {
        // 设置当前租户ID
        entity.TenantId = repository.CurrentTenantId;
        
        // 插入实体
        await repository.InsertAsync(entity, token);
        return entity;
    }
    
    /// <summary>
    /// 为当前租户更新实体
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="repository">租户仓储</param>
    /// <param name="entity">要更新的实体</param>
    /// <param name="token">取消令牌</param>
    /// <returns>更新后的实体</returns>
    public static async Task<TEntity> UpdateForCurrentTenantAsync<TEntity>(
        this ITenantRepository<TEntity, long, long> repository,
        TEntity entity,
        CancellationToken token = default)
        where TEntity : class, IEntity<long>, ITenant<long>
    {
        // 确保租户ID正确
        entity.TenantId = repository.CurrentTenantId;
        
        // 更新实体
        await repository.UpdateAsync(entity, token);
        return entity;
    }
    
    /// <summary>
    /// 删除实体
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="repository">租户仓储</param>
    /// <param name="entity">要删除的实体</param>
    /// <param name="token">取消令牌</param>
    /// <returns>受影响的行数</returns>
    public static async Task<int> DeleteAsync<TEntity>(
        this ITenantRepository<TEntity, long, long> repository,
        TEntity entity,
        CancellationToken token = default)
        where TEntity : class, IEntity<long>, ITenant<long>
    {
        // 删除实体
        return await repository.RemoveAsync(entity, token);
    }
}
