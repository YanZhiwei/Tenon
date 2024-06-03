using System.Linq.Expressions;

namespace Tenon.Repository;

public interface IRepository<TEntity, in TKey> where TEntity : IEntity<TKey>
{
    Task<int> InsertAsync(TEntity entity, CancellationToken token = default);
    Task<int> InsertAsync(IEnumerable<TEntity> entities, CancellationToken token = default);
    Task<int> UpdateAsync(TEntity entity, CancellationToken token = default);
    Task<int> UpdateAsync(IEnumerable<TEntity> entities, CancellationToken token = default);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken token = default);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken token = default);
    Task<int> RemoveAsync(TEntity entity, CancellationToken token = default);
    Task<int> RemoveAsync(TKey keyValue, CancellationToken token = default);
    Task<int> RemoveAsync(IEnumerable<TEntity> entities, CancellationToken token = default);

    Task<int> UpdateAsync(TEntity entity, Expression<Func<TEntity, object>>[] updatingExpressions,
        CancellationToken token = default);

    Task<IEnumerable<TEntity>> GetAllAsync();

    Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> whereExpression,
        CancellationToken token = default);

    Task<TEntity> GetAsync(TKey keyValue, CancellationToken token = default);

    Task<TEntity> GetAsync(TKey keyValue,
        IEnumerable<Expression<Func<TEntity, dynamic>>> navigationPropertyPaths = null,
        CancellationToken token = default);

    Task<TEntity> GetAsync(TKey keyValue, Expression<Func<TEntity, dynamic>> navigationPropertyPath = null,
        CancellationToken token = default);
}