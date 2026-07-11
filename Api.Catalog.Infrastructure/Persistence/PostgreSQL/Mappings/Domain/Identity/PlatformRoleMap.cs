using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class PlatformRoleMap : BaseMap<PlatformRole>
{
    public override void Configure(EntityTypeBuilder<PlatformRole> builder)
    {
        base.Configure(builder);
        builder.ToTable("platform_role");

        builder.OwnsOne(r => r.RoleInfo, ri =>
        {
            ri.Property(p => p.Name)
                .HasColumnName("name")
                .HasMaxLength(30)
                .IsRequired();

            ri.Property(p => p.Description)
                .HasColumnName("description")
                .HasMaxLength(60)
                .IsRequired();

            ri.OwnsMany(ri => ri.Permissions, permission =>
            {
                permission.ToTable("platform_role_permission");
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
        });
        builder.Navigation(n => n.RoleInfo)
            .HasField("_roleInfo")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
