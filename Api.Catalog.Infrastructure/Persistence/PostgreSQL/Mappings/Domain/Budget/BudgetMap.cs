using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class BudgetMap : TenantScopedMap<Budget>
{
    public override void Configure(EntityTypeBuilder<Budget> builder)
    {
        base.Configure(builder);
        builder.ToTable("budget");

        builder.Property(p => p.ValidUntil).HasColumnName("valid_until");
        builder.Property(p => p.UserEmail).HasColumnName("user_email");
        builder.Property(p => p.PersonId).HasColumnName("user_id");

        builder.HasMany(i => i.Items)
            .WithOne()
            .HasForeignKey(fk => fk.BudgetId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(n => n.Items)
            .HasField("_items")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne(p => p.Person)
            .WithMany()
            .HasForeignKey(fk => fk.PersonId);
        builder.Navigation(n => n.Person)
            .HasField("_person")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
