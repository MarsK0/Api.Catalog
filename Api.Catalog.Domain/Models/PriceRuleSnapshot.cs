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
    public static Result<PriceRuleSnapshot> Create(
        Guid priceRuleId,
        EPriceRuleType ruleType,
        decimal price
    )
    {
        if (priceRuleId == Guid.Empty)
            return DomainResultFailures.Validation("Um id de regra de preço deve ser informada para a snapshot da regra de preço.");

        if (!Enum.IsDefined(ruleType))
            return DomainResultFailures.Validation("Tipo de regra de preço inválida.");

        if (price < 0)
            return DomainResultFailures.Validation("O preço informado deve ser maior ou igual a 0.");

        return new PriceRuleSnapshot(priceRuleId, ruleType, price);
    }
};

