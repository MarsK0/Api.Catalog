using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal sealed class AuditLogMap : BaseMap<AuditLog>
{
    public override void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        base.Configure(builder);
        builder.ToTable("audit_log");

        builder.Property(p => p.EntityName).HasColumnName("entity_name").HasMaxLength(64);
        builder.Property(p => p.EntityId).HasColumnName("entity_id");
        builder.Property(p => p.Action).HasColumnName("action");
        builder.Property(p => p.Changes).HasColumnName("changes");
        builder.Property(p => p.Success).HasColumnName("success");
        builder.Property(p => p.OccurredAt).HasColumnName("ocurred_at");
        builder.Property(p => p.TenantId).HasColumnName("tenant_id");
        builder.Property(p => p.PersonId).HasColumnName("user_id");

        builder.HasOne(o => o.Tenant)
            .WithMany()
            .HasForeignKey(fk => fk.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(n => n.Tenant)
            .HasField("_tenant")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne(o => o.Person)
            .WithMany()
            .HasForeignKey(fk => fk.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(n => n.Person)
            .HasField("_person")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(i => new { i.TenantId, i.EntityName, i.EntityId });
        builder.HasIndex(i => new { i.TenantId, i.OccurredAt });
    }
}
