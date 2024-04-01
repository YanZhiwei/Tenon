using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Tenon.Repository.EfCore.Transaction;

public abstract class UnitOfWork : IUnitOfWork
{
    protected readonly DbContext DbContext;

    protected UnitOfWork(DbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    protected IDbContextTransaction?
        DbTransaction { get; set; }

    public IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        if (DbContext.Database.CurrentTransaction is not null)
            throw new InvalidOperationException($"{nameof(DbContext.Database.CurrentTransaction)} is not null");
        DbTransaction = GetDbTransaction(isolationLevel);
        return DbTransaction;
    }

    public void Rollback()
    {
        if (DbTransaction is null)
            throw new InvalidOperationException($"{nameof(DbContext.Database.CurrentTransaction)} is null");
        DbTransaction.Rollback();
    }

    public void Commit()
    {
        if (DbTransaction is null)
            throw new InvalidOperationException($"{nameof(DbContext.Database.CurrentTransaction)} is null");
        DbTransaction.Commit();
    }

    public async Task RollbackAsync(CancellationToken token = default)
    {
        if (DbTransaction is null)
            throw new InvalidOperationException($"{nameof(DbContext.Database.CurrentTransaction)} is null");
        await DbTransaction.RollbackAsync(token);
    }

    public async Task CommitAsync(CancellationToken token = default)
    {
        if (DbTransaction is null)
            throw new InvalidOperationException($"{nameof(DbContext.Database.CurrentTransaction)} is null");
        await DbTransaction.CommitAsync(token);
    }

    protected abstract IDbContextTransaction GetDbTransaction(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
}