using System.Linq.Expressions;

namespace Tenon.Repository.EfCore;

public interface IEfRepository<TEntity, in TKey> where TEntity : IEntity<TKey>
{
    IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression, bool noTracking = true);

    Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> whereExpression, bool noTracking = true,
        CancellationToken token = default);

    Task<TEntity?> GetAsync(TKey keyValue, bool noTracking = true, CancellationToken token = default);

    Task<TEntity?> GetAsync(TKey keyValue,
        IEnumerable<Expression<Func<TEntity, dynamic>>> navigationPropertyPaths = null, bool noTracking = true,
        CancellationToken token = default);

    Task<TEntity?> GetAsync(TKey keyValue, Expression<Func<TEntity, dynamic>> navigationPropertyPath = null,
        bool noTracking = true, CancellationToken token = default);

    IQueryable<TEntity> GetAll(bool noTracking = true);
}