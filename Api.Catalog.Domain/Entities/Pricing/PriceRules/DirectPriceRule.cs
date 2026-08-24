using Api.Catalog.Domain.Enums;
using Api.Catalog.Domain.Models;

namespace Api.Catalog.Domain.Entities;

public sealed class DirectPriceRule : BasePriceRule
{
    public override EPriceRuleType RuleType => EPriceRuleType.Direct;
    private DirectPriceRule() { }
    public static Result<DirectPriceRule> Create(
        Guid priceListId,
        Guid productId,
        decimal price
    )
    {
        var validation = ValidateBase(priceListId, productId, price);
        if (!validation.IsSuccess)
            return validation.Failure;

        return new DirectPriceRule
        {
            PriceListId = priceListId,
            ProductId = productId,
            Price = price
        };
    }

    public override bool AppliesTo(PricingContext _) => true;
}
