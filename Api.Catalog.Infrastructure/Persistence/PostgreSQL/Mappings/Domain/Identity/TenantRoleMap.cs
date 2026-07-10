using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class TenantRoleMap : TenantScopedMap<TenantRole>
{
    public override void Configure(EntityTypeBuilder<TenantRole> builder)
    {
        base.Configure(builder);
        builder.ToTable("tenant_role");

        builder.OwnsOne(r => r.RoleInfo, ri =>
        {
            ri.Property(p => p.Name)
                .HasColumnName("name")
                .HasMaxLength(30);

            ri.Property(p => p.Description)
                .HasColumnName("description")
                .HasMaxLength(60);

            ri.Property(p => p.Permissions)
                .HasColumnName("permissions")
                .HasColumnType("text[]");
        });
        builder.Navigation(n => n.RoleInfo)
            .HasField("_roleInfo")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
