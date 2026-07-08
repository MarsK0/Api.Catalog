using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class DirectPriceRuleMap : BasePriceRuleMap<DirectPriceRule>
{
    public override void Configure(EntityTypeBuilder<DirectPriceRule> builder)
    {
        base.Configure(builder);
        builder.ToTable("price_rule_direct");
    }
}
