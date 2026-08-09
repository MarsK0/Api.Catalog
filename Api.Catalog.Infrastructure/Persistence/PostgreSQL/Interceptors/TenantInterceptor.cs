using Api.Catalog.Application.Contracts.Contexts;
using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

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
        if (eventData.Context is not null && tenantContext.TenantId is Guid)
            ApplyTenantId(eventData.Context, tenantContext.TenantId);

        return base.SavingChanges(eventData, result);
    }
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct
    )
    {
        if (eventData.Context is not null && tenantContext.TenantId is Guid)
            ApplyTenantId(eventData.Context, tenantContext.TenantId);

        return base.SavingChangesAsync(eventData, result, ct);
    }
    private void ApplyTenantId(DbContext context, Guid? tenantId)
    {
        foreach (var entry in context.ChangeTracker.Entries<TenantScopedEntity>())
            if (entry.State == EntityState.Added && entry.Entity.TenantId == Guid.Empty)
                TenantAssigner.Assign(
                    entry.Entity,
                    tenantId ?? throw new InvalidOperationException(
                        "Tenant obrigatório para a operação de definição de tenantId. Revisar o contexto de tenant")
                );
    }
}

internal static class TenantAssigner
{
    private static readonly Action<TenantScopedEntity, Guid> _setTenant = BuildSetter();
    public static void Assign(TenantScopedEntity entity, Guid tenantId)
        => _setTenant(entity, tenantId);

    private static Action<TenantScopedEntity, Guid> BuildSetter()
    {
        var property = typeof(TenantScopedEntity).GetProperty(
            nameof(TenantScopedEntity.TenantId),
            BindingFlags.Public | BindingFlags.Instance
        ) ?? throw new InvalidOperationException("Propriedade TenantId não encontrada em TenantScopedEntity.");

        var setMethod = property.GetSetMethod(nonPublic: true)
            ?? throw new InvalidOperationException("Setter de TenantId não encontrado (private set esperado).");

        var entityParam = Expression.Parameter(typeof(TenantScopedEntity), "entity");
        var valueParam = Expression.Parameter(typeof(Guid), "tenantId");

        var call = Expression.Call(entityParam, setMethod, valueParam);

        return Expression
            .Lambda<Action<TenantScopedEntity, Guid>>(call, entityParam, valueParam)
            .Compile();
    }
}