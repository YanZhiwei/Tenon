using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Data.Common;

namespace Tenon.Repository.EfCore.Transaction;

public interface IUnitOfWork
{
    IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

    void Rollback();

    void Commit();

    Task RollbackAsync(CancellationToken token = default);

    Task CommitAsync(CancellationToken token = default);
}