using Api.Catalog.Domain.Entities;
using Api.Catalog.Domain.Enums;

namespace Api.Catalog.Domain.Models;

public record PriceRuleSnapshot
{
    public Guid PriceRuleId { get; init; }
    public EPriceRuleType RuleType { get; init; }
    public decimal Price { get; init; }
    private PriceRuleSnapshot(
        Guid priceRuleId,
        EPriceRuleType ruleType,
        decimal price
    )
    {
        PriceRuleId = priceRuleId;
        RuleType = ruleType;
        Price = price;
    }
    public static Result<PriceRuleSnapshot> Create(BasePriceRule priceRule)
    {
        if (priceRule is null)
            return DomainResultFailures.Validation("Informe uma regra de preço para a snapshot.");

        return new PriceRuleSnapshot(priceRule.Id, priceRule.RuleType, priceRule.Price);
    }
};

