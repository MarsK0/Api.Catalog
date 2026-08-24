using Api.Catalog.Domain.Models;

namespace Api.Catalog.Domain.Entities;

public class Product : TenantScopedEntity
{
    public string Description { get; private set; } = null!;
    public string? Reference { get; private set; }

    private Product() { }

    public static Result<Product> Create(
        string description,
        string? reference = null
    )
    {
        if (string.IsNullOrWhiteSpace(description))
            return DomainResultFailures.Validation("Uma descrição deve ser informada para o produto.");
        return new Product
        {
            Description = description,
            Reference = reference
        };
    }
}
