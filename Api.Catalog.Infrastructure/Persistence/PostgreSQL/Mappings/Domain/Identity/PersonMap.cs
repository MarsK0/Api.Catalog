using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class PersonMap : TenantScopedMap<Person>
{
    public override void Configure(EntityTypeBuilder<Person> builder)
    {
        base.Configure(builder);
        builder.ToTable("person");

        builder.Property(p => p.Name).HasColumnName("name").HasMaxLength(60);
        builder.Property(p => p.Email).HasColumnName("email").HasMaxLength(254);
        builder.Property(p => p.Phone).HasColumnName("phone").HasMaxLength(20);

        builder.HasIndex(i => new { i.Email, i.TenantId })
            .IsUnique();

        builder.HasMany(p => p.Roles)
            .WithMany()
            .UsingEntity<PersonRole>(
                pr => pr.HasOne<Role>().WithMany().HasForeignKey(fk => fk.RoleId),
                pr => pr.HasOne<Person>().WithMany().HasForeignKey(fk => fk.PersonId)
            );
        builder.Navigation(n => n.Roles)
            .HasField("_roles")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
