using Api.Catalog.Application.Contracts.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal sealed class AuditLogInterceptor(
    TimeProvider timeProvider,
    IPersonContext personContext,
    ITenantContext tenantContext,
    IDbContextFactory<AuditDbContext> auditDbCtxFactory,
    ILogger<AuditLogInterceptor> logger
) : SaveChangesInterceptor
{
    private sealed record PendingAudit(
        string EntityName,
        Guid? EntityId,
        EAuditAction Action,
        string ChangesJson
    );
    private List<PendingAudit>? _pending;
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
    )
    {
        if (eventData.Context is not null)
            ApplySuccessAudit(eventData.Context);

        return base.SavingChanges(eventData, result);
    }
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct
    )
    {
        if (eventData.Context is not null)
            ApplySuccessAudit(eventData.Context);

        return base.SavingChangesAsync(eventData, result, ct);
    }
    private void ApplySuccessAudit(DbContext context)
    {
        var entries = BuildEntries(context);
        _pending = entries;

        if (entries.Count == 0)
            return;

        var now = timeProvider.GetUtcNow();

        foreach (var e in entries)
            context.Add(new AuditLog
            {
                EntityName = e.EntityName,
                EntityId = e.EntityId,
                Action = e.Action,
                Changes = e.ChangesJson,
                Success = true,
                ErrorMessage = null,
                OccurredAt = now,
                TenantId = tenantContext.TenantId,
                PersonId = personContext.PersonId
            });
    }
    private static List<PendingAudit> BuildEntries(DbContext context)
    {
        var list = new List<PendingAudit>();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Metadata.IsOwned())
                continue;
            if (entry.Entity is AuditLog)
                continue;

            var isSoftDeleted = entry.State == EntityState.Modified && IsSoftDeleted(entry);

            EAuditAction? action = entry.State switch
            {
                EntityState.Added => EAuditAction.Created,
                EntityState.Deleted => EAuditAction.Deleted,
                EntityState.Modified when isSoftDeleted => EAuditAction.SoftDeleted,
                EntityState.Modified => EAuditAction.Updated,
                _ => null
            };

            if (action is null)
                continue;

            var changes = new Dictionary<string, object?>();

            foreach (var prop in entry.Properties)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        changes[prop.Metadata.Name] = prop.CurrentValue;
                        break;
                    case EntityState.Deleted:
                        changes[prop.Metadata.Name] = prop.OriginalValue;
                        break;
                    case EntityState.Modified when isSoftDeleted && prop.IsModified:
                        changes[prop.Metadata.Name] = prop.OriginalValue;
                        break;
                    case EntityState.Modified when prop.IsModified:
                        changes[prop.Metadata.Name] = new { Old = prop.OriginalValue, New = prop.CurrentValue };
                        break;
                }
            }

            list.Add(new PendingAudit(
                entry.Metadata.ClrType.Name,
                ResolveEntityId(entry),
                action.Value,
                JsonSerializer.Serialize(changes)
            ));
        }

        return list;
    }
    private static Guid? ResolveEntityId(EntityEntry entry)
    {
        var pk = entry.Metadata.FindPrimaryKey();

        if (pk is null || pk.Properties.Count != 1)
            return null;

        var value = entry.Property(pk.Properties[0].Name).CurrentValue;
        return value as Guid?;
    }
    private static bool IsSoftDeleted(EntityEntry entry) =>
        entry.Metadata.FindProperty(TrackingMetadata.DeletedAt) is not null &&
        entry.Property(TrackingMetadata.DeletedAt).CurrentValue is not null;
    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        PersistFailure(eventData.Exception);
        base.SaveChangesFailed(eventData);
    }
    private void PersistFailure(Exception ex)
    {
        if (_pending is null || !_pending.Any())
            return;

        try
        {
            using var auditCtx = auditDbCtxFactory.CreateDbContext();
            AddFailureLogs(auditCtx, _pending, ex.Message);
            auditCtx.SaveChanges();
        }
        catch
        {
            logger.LogError("Um erro ocorreu ao auditar uma operação no banco: {OriginalErrorMessage}", ex.Message);
        }
        finally
        {
            _pending = null;
        }
    }
    private void AddFailureLogs(AuditDbContext auditCtx, List<PendingAudit> entries, string errorMessage)
    {
        var now = timeProvider.GetUtcNow();

        foreach (var e in entries)
        {
            auditCtx.Add(new AuditLog
            {
                EntityName = e.EntityName,
                EntityId = e.EntityId,
                Action = e.Action,
                Changes = e.ChangesJson,
                Success = false,
                ErrorMessage = errorMessage,
                OccurredAt = now,
                TenantId = tenantContext.TenantId,
                PersonId = personContext.PersonId
            });
        }
    }
    public override async Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken ct)
    {
        await PersistFailuresAsync(eventData.Exception, ct);
        await base.SaveChangesFailedAsync(eventData, ct);
    }
    private async Task PersistFailuresAsync(Exception ex, CancellationToken ct)
    {
        if (_pending is null || !_pending.Any())
            return;

        try
        {
            using var auditCtx = auditDbCtxFactory.CreateDbContext();
            AddFailureLogs(auditCtx, _pending, ex.Message);
            await auditCtx.SaveChangesAsync(ct);
        }
        catch
        {
            logger.LogError("Um erro ocorreu ao auditar uma operação no banco: {OriginalErrorMessage}", ex.Message);
        }
        finally
        {
            _pending = null;
        }
    }
}
