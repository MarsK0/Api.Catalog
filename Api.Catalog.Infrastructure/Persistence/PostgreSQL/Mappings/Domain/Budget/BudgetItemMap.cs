using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class BudgetItemMap : TenantScopedMap<BudgetItem>
{
    public override void Configure(EntityTypeBuilder<BudgetItem> builder)
    {
        base.Configure(builder);
        builder.ToTable("budget_item");

        builder.Property(p => p.BudgetId).HasColumnName("budget_id");
        builder.Property(p => p.Quantity).HasColumnName("quantity").HasPrecision(19, 4);
        builder.Property(p => p.ProductId).HasColumnName("product_id");
        builder.Property(p => p.ProductDescription).HasColumnName("product_description").HasMaxLength(60);
        builder.Property(p => p.ProductReference).HasColumnName("product_reference").HasMaxLength(30);
        builder.Property(p => p.PriceRuleId).HasColumnName("price_rule_id");
        builder.Property(p => p.PriceRuleType).HasColumnName("price_rule_type");
        builder.Property(p => p.Price).HasColumnName("price").HasPrecision(19, 4);
    }
}
