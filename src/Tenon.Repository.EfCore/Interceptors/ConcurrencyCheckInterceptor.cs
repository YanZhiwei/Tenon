using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Tenon.Repository.EfCore.Interceptors;

public sealed class ConcurrencyCheckInterceptor : SaveChangesInterceptor
{
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

    private static void BeforeSaveChanges(DbContext dbContext)
    {
        foreach (var entry in dbContext.ChangeTracker.Entries<IConcurrency>())
        {
            var entity = entry.Entity;
            if (entry.State == EntityState.Unchanged) continue;

            entity.RowVersion = Guid.NewGuid().ToByteArray();
        }
    }
}