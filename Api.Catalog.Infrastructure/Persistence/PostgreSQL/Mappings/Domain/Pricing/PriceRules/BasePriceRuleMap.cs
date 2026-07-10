using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal abstract class BasePriceRuleMap<TRule> : TenantScopedMap<TRule> where TRule : BasePriceRule
{
    public override void Configure(EntityTypeBuilder<TRule> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.PriceListId).HasColumnName("price_list_id");
        builder.Property(p => p.ProductId).HasColumnName("product_id");
        builder.Property(p => p.Price).HasColumnName("price").HasPrecision(19, 4);

        builder.HasOne<PriceList>()
            .WithMany()
            .HasForeignKey(fk => fk.PriceListId);
    }
}
