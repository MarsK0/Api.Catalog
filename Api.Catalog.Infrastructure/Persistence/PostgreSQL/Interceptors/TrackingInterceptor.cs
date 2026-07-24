using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL.Interceptors;

internal sealed class TrackingInterceptor(
    TimeProvider timeProvider
) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
    )
    {
        if (eventData.Context is not null)
            ApplyTracking(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct
    )
    {
        if (eventData.Context is not null)
            ApplyTracking(eventData.Context);

        return base.SavingChangesAsync(eventData, result, ct);
    }

    private void ApplyTracking(DbContext context)
    {
        var now = timeProvider.GetUtcNow();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Metadata.IsOwned())
                continue;

            if (entry.State == EntityState.Added)
                entry.Property(TrackingMetadata.CreatedAt).CurrentValue = now;
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(TrackingMetadata.CreatedAt).IsModified = false;
                entry.Property(TrackingMetadata.UpdatedAt).CurrentValue = now;
            }
            else if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Property(TrackingMetadata.UpdatedAt).CurrentValue = now;
                entry.Property(TrackingMetadata.DeletedAt).CurrentValue = now;
            }
        }
    }
}
