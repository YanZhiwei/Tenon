using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tenon.Repository;

namespace Tenon.Repository.EfCore;

/// <summary>
/// EF Core 仓储实现
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
public partial class EfRepository<TEntity> : IRepository<TEntity, long>, IEfRepository<TEntity, long>
    where TEntity : EfEntity, new()
{
    protected readonly DbContext DbContext;
    private readonly ILogger<EfRepository<TEntity>> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    /// <param name="logger">日志记录器</param>
    public EfRepository(DbContext dbContext, ILogger<EfRepository<TEntity>> logger)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger;
    }

    /// <summary>
    /// 获取数据库集合
    /// </summary>
    /// <param name="noTracking">是否不追踪实体</param>
    protected virtual IQueryable<TEntity> GetDbSet(bool noTracking)
    {
        return noTracking ? DbContext.Set<TEntity>().AsNoTracking() : DbContext.Set<TEntity>();
    }
}
