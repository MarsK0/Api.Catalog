using Api.Catalog.Domain.Models;
using Api.Catalog.Domain.ValueObjects;

namespace Api.Catalog.Domain.Entities;

public class BudgetItem : TenantScopedEntity
{
    public Guid BudgetId { get; private set; }
    public decimal Quantity { get; private set; }
    public ProductSnapshot ProductSnapshot { get; private set; } = null!;
    public PriceRuleSnapshot PriceRuleSnapshot { get; private set; } = null!;
    private BudgetItem() { }

    public static Result<BudgetItem> Create(
        Guid budgetId,
        decimal quantity,
        ProductSnapshot productSnapshot,
        PriceRuleSnapshot priceRuleSnapshot
    )
    {
        if (budgetId == Guid.Empty)
            return DomainResultFailures.Validation("Um orçamento deve ser informado para a inclusão do item.");

        if (quantity <= 0)
            return DomainResultFailures.Validation("O item deve possuir uma quantidade maior que zero informada.");

        if (productSnapshot is null)
            return DomainResultFailures.Validation("Uma snapshot do produto deve ser informada para o item.");

        if (priceRuleSnapshot is null)
            return DomainResultFailures.Validation("Uma snapshot da regra de preço deve ser informada para o item.");

        return new BudgetItem
        {
            BudgetId = budgetId,
            Quantity = quantity,
            ProductSnapshot = productSnapshot,
            PriceRuleSnapshot = priceRuleSnapshot
        };
    }
}
