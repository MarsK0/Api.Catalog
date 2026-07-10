using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class ProductMap : TenantScopedMap<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);
        builder.ToTable("product");

        builder.Property(p => p.Description).HasColumnName("description").HasMaxLength(60);
        builder.Property(p => p.Reference).HasColumnName("reference").HasMaxLength(30);
    }
}
