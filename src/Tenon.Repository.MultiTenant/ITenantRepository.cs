using System.Linq.Expressions;

namespace Tenon.Repository;

/// <summary>
/// 定义多租户仓储接口
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TKey">主键类型</typeparam>
/// <typeparam name="TTenantKey">租户主键类型</typeparam>
public interface ITenantRepository<TEntity, in TKey, TTenantKey> : IRepository<TEntity, TKey> 
    where TEntity : IEntity<TKey>, ITenant<TTenantKey>
{
    /// <summary>
    /// 获取或设置当前租户标识
    /// </summary>
    TTenantKey CurrentTenantId { get; set; }
    
    /// <summary>
    /// 异步获取指定租户下的所有实体
    /// </summary>
    Task<IEnumerable<TEntity>> GetAllByTenantAsync(TTenantKey tenantId, CancellationToken token = default);
    
    /// <summary>
    /// 异步获取当前租户下的所有实体
    /// </summary>
    Task<IEnumerable<TEntity>> GetAllForCurrentTenantAsync(CancellationToken token = default);
    
    /// <summary>
    /// 异步获取指定租户下满足条件的实体列表
    /// </summary>
    Task<IEnumerable<TEntity>> GetListByTenantAsync(TTenantKey tenantId, Expression<Func<TEntity, bool>> whereExpression, CancellationToken token = default);
    
    /// <summary>
    /// 异步获取当前租户下满足条件的实体列表
    /// </summary>
    Task<IEnumerable<TEntity>> GetListForCurrentTenantAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken token = default);
    
    /// <summary>
    /// 异步检查指定租户下是否存在满足条件的实体
    /// </summary>
    Task<bool> AnyByTenantAsync(TTenantKey tenantId, Expression<Func<TEntity, bool>> whereExpression, CancellationToken token = default);
    
    /// <summary>
    /// 异步检查当前租户下是否存在满足条件的实体
    /// </summary>
    Task<bool> AnyForCurrentTenantAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken token = default);
    
    /// <summary>
    /// 异步获取指定租户下满足条件的实体数量
    /// </summary>
    Task<long> CountByTenantAsync(TTenantKey tenantId, Expression<Func<TEntity, bool>> whereExpression, CancellationToken token = default);
    
    /// <summary>
    /// 异步获取当前租户下满足条件的实体数量
    /// </summary>
    Task<long> CountForCurrentTenantAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken token = default);
    
    /// <summary>
    /// 异步根据主键获取指定租户下的实体
    /// </summary>
    Task<TEntity?> GetByTenantAsync(TTenantKey tenantId, TKey keyValue, CancellationToken token = default);
    
    /// <summary>
    /// 异步根据主键获取当前租户下的实体
    /// </summary>
    Task<TEntity?> GetForCurrentTenantAsync(TKey keyValue, CancellationToken token = default);
    
    /// <summary>
    /// 异步根据主键和导航属性路径获取指定租户下的实体
    /// </summary>
    Task<TEntity?> GetByTenantAsync(TTenantKey tenantId, TKey keyValue, 
        IEnumerable<Expression<Func<TEntity, dynamic>>>? navigationPropertyPaths = null, 
        CancellationToken token = default);
    
    /// <summary>
    /// 异步根据主键和导航属性路径获取当前租户下的实体
    /// </summary>
    Task<TEntity?> GetForCurrentTenantWithNavigationAsync(TKey keyValue, 
        IEnumerable<Expression<Func<TEntity, dynamic>>>? navigationPropertyPaths = null, 
        CancellationToken token = default);
}
