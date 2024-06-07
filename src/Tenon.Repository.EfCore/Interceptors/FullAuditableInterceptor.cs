using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Tenon.Repository.EfCore.Interceptors;

public class FullAuditableInterceptor(EfAuditableUser auditable) : SaveChangesInterceptor
{
    private readonly EfAuditableUser _auditable = auditable ?? throw new ArgumentNullException(nameof(auditable));


    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ArgumentNullException.ThrowIfNull(eventData.Context);
        BeforeSaveChanges(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        ArgumentNullException.ThrowIfNull(eventData.Context);
        BeforeSaveChanges(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void BeforeSaveChanges(DbContext dbContext)
    {
        foreach (var entry in dbContext.ChangeTracker.Entries<EfFullAuditableEntity>())
        {
            var entity = entry.Entity;
            var isSoftDelete = false;
            if (entity is ISoftDelete softDelete) isSoftDelete = softDelete.IsDeleted;
            switch (entry.State)
            {
                case EntityState.Added:
                    entity.CreatedAt = DateTimeOffset.UtcNow;
                    entity.CreatedBy = _auditable.User;
                    break;

                case EntityState.Modified or EntityState.Deleted:
                    if (isSoftDelete == false)
                    {
                        entity.UpdatedAt = DateTimeOffset.UtcNow;
                        entity.UpdatedBy = _auditable.User;
                    }
                    else
                    {
                        entity.DeletedAt = DateTimeOffset.UtcNow;
                        entity.DeletedBy = _auditable.User;
                    }

                    break;
            }
        }
    }
}