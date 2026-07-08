using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class TenantScopedMap<TEntity> : BaseMap<TEntity> where TEntity : TenantScopedEntity
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);
        builder.Property(p => p.TenantId).HasColumnName("tenant_id");

        builder.HasOne(o => o.Tenant)
            .WithMany()
            .HasForeignKey(fk => fk.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(n => n.Tenant)
            .HasField("_tenant")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
