using Api.Catalog.Application.Entities;
using Api.Catalog.Application.Entities.Platform;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal sealed class PlatformUserMap : BaseMap<PlatformUser>
{
    public override void Configure(EntityTypeBuilder<PlatformUser> builder)
    {
        base.Configure(builder);
        builder.ToTable("platform_user");

        builder.Property(p => p.Login).HasColumnName("login").HasMaxLength(30);
        builder.Property(p => p.Name).HasColumnName("name").HasMaxLength(60);
        builder.Property(p => p.PasswordHash).HasColumnName("password_hash");
        builder.Property(p => p.Email).HasColumnName("email");

        builder.HasIndex(i => i.Email)
            .IsUnique();
        builder.HasIndex(i => i.Login)
            .IsUnique();

        builder.HasMany(p => p.Roles)
            .WithMany()
            .UsingEntity<PlatformUserRole>(
                pr => pr.HasOne<PlatformRole>().WithMany().HasForeignKey(fk => fk.RoleId),
                pr => pr.HasOne<PlatformUser>().WithMany().HasForeignKey(fk => fk.UserId)
            );
        builder.Navigation(n => n.Roles)
            .HasField("_roles")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

    }
}
