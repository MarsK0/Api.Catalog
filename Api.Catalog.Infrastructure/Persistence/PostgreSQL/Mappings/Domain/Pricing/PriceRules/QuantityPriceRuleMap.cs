using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class QuantityPriceRuleMap : BasePriceRuleMap<QuantityPriceRule>
{
    public override void Configure(EntityTypeBuilder<QuantityPriceRule> builder)
    {
        base.Configure(builder);
        builder.ToTable("price_rule_quantity");

        builder.Property(p => p.Min).HasColumnName("min");
        builder.Property(p => p.Max).HasColumnName("max");
    }
}
