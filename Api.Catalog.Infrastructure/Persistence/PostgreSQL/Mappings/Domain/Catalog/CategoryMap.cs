using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class CategoryMap : TenantScopedMap<Category>
{
    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        base.Configure(builder);
        builder.ToTable("category");

        builder.Property(p => p.Description).HasColumnName("description").HasMaxLength(60);

        builder.HasMany(c => c.SubCategories)
            .WithOne()
            .HasForeignKey(fk => fk.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(n => n.SubCategories)
            .HasField("_subCategories")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
