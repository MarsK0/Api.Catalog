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

            ri.OwnsMany(m => m.Permissions, permission =>
            {
                permission.ToTable("tenant_role_permission");
                permission.Property<Guid>("Id").ValueGeneratedOnAdd();
                permission.HasKey("Id");

                permission.WithOwner().HasForeignKey("platform_role_id");

                permission.Property(p => p.Scope)
                    .HasColumnName("scope")
                    .HasMaxLength(10);

                permission.Property(p => p.Resource)
                    .HasColumnName("resource")
                    .HasMaxLength(30);

                permission.Property(p => p.Action)
                    .HasColumnName("action")
                    .HasMaxLength(30);
            });

            ri.Navigation(n => n.Permissions)
                .HasField("_permissions")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });
        builder.Navigation(n => n.RoleInfo)
            .HasField("_roleInfo")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
