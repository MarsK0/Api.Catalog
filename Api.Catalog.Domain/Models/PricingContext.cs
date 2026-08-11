using Api.Catalog.Domain.ValueObjects;

namespace Api.Catalog.Domain.Models;

public record PricingContext
{
    public PersonSnapshot? Person { get; init; }
    public decimal Quantity { get; init; }
    private PricingContext(PersonSnapshot? person, decimal quantity)
    {
        Person = person;
        Quantity = quantity;
    }

    public static Result<PricingContext> Create(PersonSnapshot? person, decimal quantity)
    {
        if (quantity <= 0)
            return DomainResultFailures.Validation("Uma quantidade maior que zero deve ser informada.");

        return new PricingContext(person, quantity);
    }
};

