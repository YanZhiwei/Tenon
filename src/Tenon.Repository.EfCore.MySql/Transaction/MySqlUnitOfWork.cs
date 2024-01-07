using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Tenon.Repository.EfCore.Transaction;

namespace Tenon.Repository.EfCore.MySql.Transaction;

public sealed class MySqlUnitOfWork(DbContext dbContext) : UnitOfWork(dbContext)
{
    protected override IDbContextTransaction GetDbTransaction(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        return DbContext.Database.BeginTransaction(isolationLevel);
    }
}