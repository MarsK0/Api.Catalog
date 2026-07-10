using Api.Catalog.Domain.Enums;
using Api.Catalog.Domain.Models;
using System.Diagnostics.CodeAnalysis;

namespace Api.Catalog.Domain.Entities;

public class BudgetItem : TenantScopedEntity
{
    public Guid BudgetId { get; private set; }
    public decimal Quantity { get; private set; }
    // Product Snapshot
    public Guid ProductId { get; private set; }
    [SuppressMessage("Compiler", "CS8618", Justification = "Populado na camada de infra")]
    private readonly Product _product = null!;
    public Product Product => _product;
    public string ProductDescription { get; private set; } = null!;
    public string? ProductReference { get; private set; } = null!;
    // PriceRule Snapshot
    public Guid PriceRuleId { get; private set; }
    public EPriceRuleType PriceRuleType { get; private set; }
    public decimal Price { get; private set; }

    private BudgetItem() { }

    public static AppResult<BudgetItem> Create(
        Guid budgetId,
        decimal quantity,
        Guid productId,
        ProductSnapshot productSnapshot,
        PriceRuleSnapshot priceRuleSnapshot
    )
    {
        return new BudgetItem
        {
            BudgetId = budgetId,
            Quantity = quantity,
            ProductId = productId,
            ProductDescription = productSnapshot.Description,
            ProductReference = productSnapshot.Reference,
            PriceRuleId = priceRuleSnapshot.PriceRuleId,
            PriceRuleType = priceRuleSnapshot.RuleType,
            Price = priceRuleSnapshot.Price,
        };
    }
}
