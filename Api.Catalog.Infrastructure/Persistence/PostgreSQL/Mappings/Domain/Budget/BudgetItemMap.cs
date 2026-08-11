using Api.Catalog.Domain.Entities;
using Api.Catalog.Infrastructure.Persistence.PostgreSQL.Extensions.ValueObjectsExtensions;
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
        builder.OwnsProductSnapshot(p => p.ProductSnapshot);
        builder.OwnsPriceRuleSnapshot(p => p.PriceRuleSnapshot);
    }
}
