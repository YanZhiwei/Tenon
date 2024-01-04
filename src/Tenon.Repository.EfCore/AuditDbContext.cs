using Microsoft.EntityFrameworkCore;

namespace Tenon.Repository.EfCore;

public abstract class AuditDbContext<TAuditKey> : DbContext
{
    protected AuditDbContext(DbContextOptions options)
        : base(options)
    {

    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var setAuditResult = SetAuditFields();
        var isManualTransaction = false;
        try
        {
            if (setAuditResult && Database is { AutoTransactionBehavior: AutoTransactionBehavior.Never, CurrentTransaction: null })
            {
                Database.AutoTransactionBehavior = AutoTransactionBehavior.WhenNeeded;
                isManualTransaction = true;
            }
            return base.SaveChangesAsync(cancellationToken);
        }
        finally
        {
            if (isManualTransaction)
                Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
        }
    }

    protected virtual bool SetAuditFields()
    {
        var curOperator = GetOperator();
        if (curOperator == null) return false;
        var allBasicAuditEntities =
            ChangeTracker.Entries<IBasicAuditInfo<TAuditKey>>().Where(x => x.State == EntityState.Added);
        foreach (var basicAuditEntity in allBasicAuditEntities)
        {
            basicAuditEntity.Entity.CreateBy = curOperator.Id;
            basicAuditEntity.Entity.CreateTime = DateTime.UtcNow;
        }

        var allFulAuditEntities = ChangeTracker.Entries<IFullAuditInfo<TAuditKey>>()
            .Where(x => x.State is EntityState.Modified or EntityState.Added);
        // ReSharper disable once PossibleMultipleEnumeration
        foreach (var fulAuditEntity in allFulAuditEntities)
        {
            fulAuditEntity.Entity.ModifyBy = curOperator.Id;
            fulAuditEntity.Entity.ModifyTime = DateTime.UtcNow;
        }

        return allFulAuditEntities.Any() || allFulAuditEntities.Any();
    }

    protected abstract Operator<TAuditKey>? GetOperator();
}