using Api.Catalog.Domain.Models;

namespace Api.Catalog.Domain.Entities;

public abstract class BasePriceRule : TenantScopedEntity
{
    public Guid PriceListId { get; protected set; }
    public Guid ProductId { get; protected set; }
    public decimal Price { get; protected set; }

    protected BasePriceRule() { }
    protected BasePriceRule(
        Guid priceListId,
        Guid productId,
        decimal price
    )
    {
        PriceListId = priceListId;
        ProductId = productId;
        Price = price;
    }

    protected static Result ValidateBase(
        Guid priceListId,
        Guid productId,
        decimal price
    )
    {
        if (priceListId == Guid.Empty)
            return DomainResultFailures.Validation("Uma lista de preço deve ser informada para a regra de preço.");
        if (productId == Guid.Empty)
            return DomainResultFailures.Validation("Um produto deve ser informado para a regra de preço.");
        if (price < decimal.Zero)
            return DomainResultFailures.Validation("O valor do preço do produto deve ser maior ou igual a zero");

        return Result.Success;
    }
    public abstract bool AppliesTo(PricingContext context);
}