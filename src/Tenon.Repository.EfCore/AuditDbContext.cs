using Microsoft.EntityFrameworkCore;

namespace Tenon.Repository.EfCore;

public abstract class AuditDbContext(DbContextOptions options, IAuditContextAccessor auditContext) : DbContext(options)
{
    private readonly IAuditContextAccessor _contextAccessor = auditContext;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected abstract long GetUserId(IAuditContextAccessor context);

    protected virtual void SetAuditFields()
    {
        var allBasicAuditEntities =
            ChangeTracker.Entries<EfBasicAuditEntity>().Where(x => x.State == EntityState.Added);
        foreach (var addedEntity in allBasicAuditEntities)
        {
            addedEntity.Entity.CreateTime = DateTime.UtcNow;
            addedEntity.Entity.CreateBy = GetUserId(_contextAccessor);
        }

        var allModifiedEntities = ChangeTracker.Entries<EfBasicAuditEntity>().Where(x => x.State == EntityState.Modified);
        foreach (var modifiedEntity in allModifiedEntities)
        {
            modifiedEntity.Entity.ModifyTime = DateTime.UtcNow;
            modifiedEntity.Entity.ModifyBy = GetUserId(_contextAccessor);
        }
    }
}