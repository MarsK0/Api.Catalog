using Api.Catalog.Application.Contracts.Contexts;
using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal sealed class TenantInterceptor(
    ITenantContext tenantContext
) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
    )
    {
        if (eventData.Context is not null)
            ApplyTenantId(eventData.Context, tenantContext.TenantId);

        return base.SavingChanges(eventData, result);
    }
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct
    )
    {
        if (eventData.Context is not null)
            ApplyTenantId(eventData.Context, tenantContext.TenantId);

        return base.SavingChangesAsync(eventData, result, ct);
    }
    private void ApplyTenantId(DbContext context, Guid? tenantId)
    {
        foreach (var entry in context.ChangeTracker.Entries<TenantScopedEntity>())
            if (entry.State == EntityState.Added && entry.Entity.TenantId == Guid.Empty)
                entry.Entity.SetTenant(tenantId
                    ?? throw new InvalidOperationException("Tenant obrigatório para a operação de definição de tenantId. Revisar o contexto de tenant"));
    }
}
