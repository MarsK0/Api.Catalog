namespace Api.Catalog.Domain.Models;

public record PricingContext
(
    PersonSnapshot? Person,
    decimal Quantity
);
public record PersonSnapshot(
    Guid Id,
    string Name
);
