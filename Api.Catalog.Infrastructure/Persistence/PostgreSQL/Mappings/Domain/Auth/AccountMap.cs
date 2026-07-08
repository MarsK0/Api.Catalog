using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class AccountMap : BaseMap<Account>
{
    public override void Configure(EntityTypeBuilder<Account> builder)
    {
        base.Configure(builder);
        builder.ToTable("account");

        builder.Property(p => p.PersonId).HasColumnName("person_id");
        builder.Property(p => p.PasswordHash).HasColumnName("password_hash").HasMaxLength(64);

        builder.HasOne(a => a.Person)
            .WithMany()
            .HasForeignKey(fk => fk.PersonId);
        builder.Navigation(n => n.Person)
            .HasField("_person")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
