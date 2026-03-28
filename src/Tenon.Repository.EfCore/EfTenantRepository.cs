using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Tenon.Repository.EfCore;

/// <summary>
/// EF Core 多租户仓储实现
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
public class EfTenantRepository<TEntity> : EfRepository<TEntity>, ITenantRepository<TEntity, long, long>, ITenantFilterable<long>
    where TEntity : EfTenantEntity, new()
{
    private readonly IEfTenantResolver _tenantResolver;
    private bool _tenantFilterEnabled = true;
    private long _currentTenantId;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    /// <param name="logger">日志记录器</param>
    /// <param name="tenantResolver">租户解析器</param>
    public EfTenantRepository(
        DbContext dbContext,
        ILogger<EfTenantRepository<TEntity>> logger,
        IEfTenantResolver tenantResolver)
        : base(dbContext, logger)
    {
        _tenantResolver = tenantResolver ?? throw new ArgumentNullException(nameof(tenantResolver));
        _currentTenantId = _tenantResolver.TenantId;
    }

    /// <summary>
    /// 获取或设置当前租户标识
    /// </summary>
    public long CurrentTenantId
    {
        get => _currentTenantId;
        set => _currentTenantId = value;
    }

    /// <summary>
    /// 获取或设置是否启用租户过滤
    /// </summary>
    public bool TenantFilterEnabled
    {
        get => _tenantFilterEnabled;
        set => _tenantFilterEnabled = value;
    }

    /// <summary>
    /// 禁用租户过滤（通过 IgnoreQueryFilters 绕过全局过滤器）
    /// </summary>
    /// <returns>一个可释放的对象，用于在完成操作后恢复租户过滤</returns>
    public IDisposable DisableTenantFilter()
    {
        var previousState = _tenantFilterEnabled;
        _tenantFilterEnabled = false;
        return new DisposeAction(() => _tenantFilterEnabled = previousState);
    }

    /// <summary>
    /// 启用租户过滤
    /// </summary>
    public void EnableTenantFilter()
    {
        _tenantFilterEnabled = true;
    }

    /// <summary>
    /// 临时切换到指定租户（修改 resolver 状态，全局过滤器动态读取）
    /// </summary>
    /// <param name="tenantId">租户标识</param>
    /// <returns>一个可释放的对象，用于在完成操作后恢复原租户</returns>
    public IDisposable ChangeTenant(long tenantId)
    {
        var previousTenantId = _tenantResolver.TenantId;
        _tenantResolver.TenantId = tenantId;
        _currentTenantId = tenantId;
        return new DisposeAction(() =>
        {
            _tenantResolver.TenantId = previousTenantId;
            _currentTenantId = previousTenantId;
        });
    }

    /// <summary>
    /// 获取数据库集合，租户过滤由 DbContext 全局查询过滤器负责。
    /// 仅在禁用租户过滤时调用 IgnoreQueryFilters 绕过全局过滤器。
    /// </summary>
    /// <param name="noTracking">是否不追踪实体</param>
    protected override IQueryable<TEntity> GetDbSet(bool noTracking)
    {
        var query = base.GetDbSet(noTracking);
        if (!_tenantFilterEnabled)
            query = query.IgnoreQueryFilters();
        return query;
    }

    /// <summary>
    /// 异步获取指定租户下的所有实体
    /// </summary>
    /// <param name="tenantId">租户标识</param>
    /// <param name="token">取消令牌</param>
    public async Task<IEnumerable<TEntity>> GetAllByTenantAsync(long tenantId, CancellationToken token = default)
    {
        using (ChangeTenant(tenantId))
        {
            return await GetAllAsync();
        }
    }

    /// <summary>
    /// 异步获取当前租户下的所有实体
    /// </summary>
    /// <param name="token">取消令牌</param>
    public async Task<IEnumerable<TEntity>> GetAllForCurrentTenantAsync(CancellationToken token = default)
    {
        return await GetAllAsync();
    }

    /// <summary>
    /// 异步获取指定租户下满足条件的实体列表
    /// </summary>
    /// <param name="tenantId">租户标识</param>
    /// <param name="whereExpression">查询条件表达式</param>
    /// <param name="token">取消令牌</param>
    public async Task<IEnumerable<TEntity>> GetListByTenantAsync(long tenantId, Expression<Func<TEntity, bool>> whereExpression, CancellationToken token = default)
    {
        using (ChangeTenant(tenantId))
        {
            return await GetListAsync(whereExpression, token);
        }
    }

    /// <summary>
    /// 异步获取当前租户下满足条件的实体列表
    /// </summary>
    /// <param name="whereExpression">查询条件表达式</param>
    /// <param name="token">取消令牌</param>
    public async Task<IEnumerable<TEntity>> GetListForCurrentTenantAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken token = default)
    {
        return await GetListAsync(whereExpression, token);
    }

    /// <summary>
    /// 异步检查指定租户下是否存在满足条件的实体
    /// </summary>
    /// <param name="tenantId">租户标识</param>
    /// <param name="whereExpression">查询条件表达式</param>
    /// <param name="token">取消令牌</param>
    public async Task<bool> AnyByTenantAsync(long tenantId, Expression<Func<TEntity, bool>> whereExpression, CancellationToken token = default)
    {
        using (ChangeTenant(tenantId))
        {
            return await AnyAsync(whereExpression, token);
        }
    }

    /// <summary>
    /// 异步检查当前租户下是否存在满足条件的实体
    /// </summary>
    /// <param name="whereExpression">查询条件表达式</param>
    /// <param name="token">取消令牌</param>
    public async Task<bool> AnyForCurrentTenantAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken token = default)
    {
        return await AnyAsync(whereExpression, token);
    }

    /// <summary>
    /// 异步获取指定租户下满足条件的实体数量
    /// </summary>
    /// <param name="tenantId">租户标识</param>
    /// <param name="whereExpression">查询条件表达式</param>
    /// <param name="token">取消令牌</param>
    public async Task<long> CountByTenantAsync(long tenantId, Expression<Func<TEntity, bool>> whereExpression, CancellationToken token = default)
    {
        using (ChangeTenant(tenantId))
        {
            return await CountAsync(whereExpression, token);
        }
    }

    /// <summary>
    /// 异步获取当前租户下满足条件的实体数量
    /// </summary>
    /// <param name="whereExpression">查询条件表达式</param>
    /// <param name="token">取消令牌</param>
    public async Task<long> CountForCurrentTenantAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken token = default)
    {
        return await CountAsync(whereExpression, token);
    }

    /// <summary>
    /// 异步根据主键获取指定租户下的实体
    /// </summary>
    /// <param name="tenantId">租户标识</param>
    /// <param name="keyValue">主键值</param>
    /// <param name="token">取消令牌</param>
    public async Task<TEntity?> GetByTenantAsync(long tenantId, long keyValue, CancellationToken token = default)
    {
        using (ChangeTenant(tenantId))
        {
            return await GetAsync(keyValue, token);
        }
    }

    /// <summary>
    /// 异步根据主键获取当前租户下的实体
    /// </summary>
    /// <param name="keyValue">主键值</param>
    /// <param name="token">取消令牌</param>
    public async Task<TEntity?> GetForCurrentTenantAsync(long keyValue, CancellationToken token = default)
    {
        return await GetAsync(keyValue, token);
    }

    /// <summary>
    /// 异步根据主键和导航属性路径获取指定租户下的实体
    /// </summary>
    /// <param name="tenantId">租户标识</param>
    /// <param name="keyValue">主键值</param>
    /// <param name="navigationPropertyPaths">导航属性路径集合</param>
    /// <param name="token">取消令牌</param>
    public async Task<TEntity?> GetByTenantAsync(long tenantId, long keyValue, 
        IEnumerable<Expression<Func<TEntity, dynamic>>>? navigationPropertyPaths = null, 
        CancellationToken token = default)
    {
        using (ChangeTenant(tenantId))
        {
            return await GetAsync(keyValue, navigationPropertyPaths, token);
        }
    }

    /// <summary>
    /// 异步根据主键和导航属性路径获取当前租户下的实体
    /// </summary>
    /// <param name="keyValue">主键值</param>
    /// <param name="navigationPropertyPaths">导航属性路径集合</param>
    /// <param name="token">取消令牌</param>
    public async Task<TEntity?> GetForCurrentTenantWithNavigationAsync(long keyValue, 
        IEnumerable<Expression<Func<TEntity, dynamic>>>? navigationPropertyPaths = null, 
        CancellationToken token = default)
    {
        return await GetAsync(keyValue, navigationPropertyPaths, token);
    }

}

