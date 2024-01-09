using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Tenon.Repository.EfCore.Transaction;

namespace Tenon.Repository.EfCore.Sqlite.Transaction;

public sealed class SqliteUnitOfWork(DbContext dbContext) : UnitOfWork(dbContext)
{
    protected override IDbContextTransaction GetDbTransaction(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        return DbContext.Database.BeginTransaction(isolationLevel);
    }
}