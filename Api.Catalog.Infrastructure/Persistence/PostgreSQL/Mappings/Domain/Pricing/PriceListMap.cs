using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class PriceListMap : TenantScopedMap<PriceList>
{
    public override void Configure(EntityTypeBuilder<PriceList> builder)
    {
        base.Configure(builder);
        builder.ToTable("pricing_list");

        builder.Property(p => p.Name).HasColumnName("name").HasMaxLength(60);
        builder.Property(p => p.ValidFrom).HasColumnName("valid_from");
        builder.Property(p => p.ValidUntil).HasColumnName("valid_until");
    }
}
