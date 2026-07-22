using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class PersonMap : BaseMap<Person>
{
    public override void Configure(EntityTypeBuilder<Person> builder)
    {
        base.Configure(builder);
        builder.ToTable("person");

        builder.Property(p => p.Name).HasColumnName("name").HasMaxLength(60);
        builder.Property(p => p.Email).HasColumnName("email").HasMaxLength(120);
        builder.Property(p => p.Phone).HasColumnName("phone").HasMaxLength(20);

        builder.HasIndex(i => i.Email)
            .IsUnique();

        builder.HasMany(p => p.TenantRoles)
            .WithMany()
            .UsingEntity<PersonTenantRole>(
                pr => pr.HasOne<TenantRole>().WithMany().HasForeignKey(fk => fk.TenantRoleId),
                pr => pr.HasOne<Person>().WithMany().HasForeignKey(fk => fk.PersonId)
            );
        builder.Navigation(n => n.TenantRoles)
            .HasField("_tenantRoles")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(p => p.PlatformRoles)
            .WithMany()
            .UsingEntity<PersonPlatformRole>(
                pr => pr.HasOne<PlatformRole>().WithMany().HasForeignKey(fk => fk.PlatformRoleId),
                pr => pr.HasOne<Person>().WithMany().HasForeignKey(fk => fk.PersonId)
            );
        builder.Navigation(n => n.PlatformRoles)
            .HasField("_platformRoles")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
