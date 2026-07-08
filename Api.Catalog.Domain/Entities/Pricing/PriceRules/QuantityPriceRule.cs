using Api.Catalog.Domain.Models;

namespace Api.Catalog.Domain.Entities;

public record ProductQuantityCondition(decimal Min, decimal Max);
public sealed class QuantityPriceRule : BasePriceRule
{
    public decimal Min { get; private set; }
    public decimal Max { get; private set; }
    private QuantityPriceRule() { }
    private QuantityPriceRule(
        Guid priceListId,
        Guid productId,
        decimal price,
        decimal min,
        decimal max
    ) : base(priceListId, productId, price)
    {
        Min = min;
        Max = max;
    }

    public static AppResult<QuantityPriceRule> Create(
        Guid priceListId,
        Guid productId,
        decimal price,
        ProductQuantityCondition condition
    )
    {
        var validation = ValidateBase(priceListId, productId, price);
        if (!validation.IsSuccess)
            return validation.Failure;

        return CreateInstance(priceListId, productId, price, condition);
    }
    public override bool AppliesTo(PricingContext context)
    {
        return (
            Min <= context.Quantity &&
            Max >= context.Quantity
        );
    }
    private static AppResult<QuantityPriceRule> CreateInstance(
        Guid priceListId,
        Guid productId,
        decimal price,
        ProductQuantityCondition condition
    )
    {
        if (condition.Min < decimal.Zero || condition.Max < decimal.Zero)
            return AppFailure.DomainValidation("As quantidades mínima e máxima devem ser maiores ou iguais a zero.");

        if (condition.Min > condition.Max)
            return AppFailure.DomainValidation("A quantidade mínima não deve ser maior do que a quantidade máxima");

        return new QuantityPriceRule(
            priceListId,
            productId,
            price,
            condition.Min,
            condition.Max
        );
    }
}
