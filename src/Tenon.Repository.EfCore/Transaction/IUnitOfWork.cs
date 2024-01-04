using System.Data;

namespace Tenon.Repository.EfCore.Transaction;

public interface IUnitOfWork
{
    void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

    void Rollback();

    void Commit();

    Task RollbackAsync(CancellationToken token = default);

    Task CommitAsync(CancellationToken token = default);
}