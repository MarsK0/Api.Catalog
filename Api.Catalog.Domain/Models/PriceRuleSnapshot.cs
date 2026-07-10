using Api.Catalog.Domain.Enums;

namespace Api.Catalog.Domain.Models;

public record PriceRuleSnapshot(
    Guid PriceRuleId,
    EPriceRuleType RuleType,
    decimal Price
);
