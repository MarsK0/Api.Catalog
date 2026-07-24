using Api.Catalog.Application.Contracts.Contexts;
using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;
using System.Reflection;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal static class AppDbContextExtensions
{
    public static ModelBuilder ApplyDataRules(this ModelBuilder modelBuilder, ITenantContext tenantContext)
    {
        // Percorre os tipos das entidades, valida se é do tipo TenantScopedEntity, cria genericamente
        // o método ApplyTenantFilter e o invoca com modelBuilder como argumento
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(TenantScopedEntity).IsAssignableFrom(entity.ClrType))
            {
                var method = typeof(AppDbContextExtensions)
                    .GetMethod(nameof(ApplyTenantFilter), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(entity.ClrType);

                method.Invoke(null, [modelBuilder, tenantContext]);
            }

            if (!entity.IsOwned() && entity.ClrType != typeof(AuditLog))
            {
                var builder = modelBuilder.Entity(entity.ClrType);
                builder.Property<DateTimeOffset>(TrackingMetadata.CreatedAt).HasColumnName("created_at");
                builder.Property<DateTimeOffset?>(TrackingMetadata.UpdatedAt).HasColumnName("updated_at");
                builder.Property<DateTimeOffset?>(TrackingMetadata.DeletedAt).HasColumnName("deleted_at");
                ApplySoftDeletedFilter(builder, entity.ClrType);
            }

        }
        return modelBuilder;
    }
    private static void ApplyTenantFilter<TEntity>(ModelBuilder modelBuilder, ITenantContext tenantContext) where TEntity : TenantScopedEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(
            entity => (
                entity.TenantId == tenantContext.TenantId ||
                tenantContext.AllowCrossTenancy
            )
        );
    }
    private static void ApplySoftDeletedFilter(EntityTypeBuilder builder, Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "entity");

        var deletedAtProperty = Expression.Call(
            typeof(EF),
            nameof(EF.Property),
            [typeof(DateTimeOffset?)],
            parameter,
            Expression.Constant(TrackingMetadata.DeletedAt)
        );

        var condition = Expression.Equal(
            deletedAtProperty,
            Expression.Constant(null, typeof(DateTimeOffset?))
        );

        var lambda = Expression.Lambda(condition, parameter);
        builder.HasQueryFilter(lambda);
    }
}
