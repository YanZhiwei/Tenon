using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Tenon.Repository.EfCore.Interceptors;

public sealed class LoggingSavingChangesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        ArgumentNullException.ThrowIfNull(eventData.Context);
        Console.WriteLine(eventData.Context.ChangeTracker.DebugView.LongView);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}