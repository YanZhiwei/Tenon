using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Tenon.Repository.EfCore.Interceptors;

public sealed class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        OnSavingChanges(eventData);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        OnSavingChanges(eventData);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void OnSavingChanges(DbContextEventData eventData)
    {
        ArgumentNullException.ThrowIfNull(eventData.Context);
        eventData.Context.ChangeTracker.DetectChanges();
        foreach (var entityEntry in eventData.Context.ChangeTracker.Entries())
            if (entityEntry is { State: EntityState.Deleted, Entity: ISoftDelete softDeleteEntity })
            {
                softDeleteEntity.IsDeleted = true;
                entityEntry.State = EntityState.Modified;
            }
    }
}