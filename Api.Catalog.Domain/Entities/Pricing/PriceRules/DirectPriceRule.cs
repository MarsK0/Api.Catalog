using Api.Catalog.Domain.Models;

namespace Api.Catalog.Domain.Entities;

public sealed class DirectPriceRule : BasePriceRule
{
    private DirectPriceRule() { }
    public static AppResult<DirectPriceRule> Create(
        Guid priceListId,
        Guid productId,
        decimal price
    )
    {
        var validation = ValidateBase(priceListId, productId, price);
        if (!validation.IsSuccess)
            return validation.Failure;

        return CreateInstance(priceListId, productId, price);
    }

    public override bool AppliesTo(PricingContext _) => true;
    private static AppResult<DirectPriceRule> CreateInstance(
        Guid priceListId,
        Guid productId,
        decimal price
    )
    {
        return new DirectPriceRule
        {
            PriceListId = priceListId,
            ProductId = productId,
            Price = price
        };
    }
}
