using Api.Catalog.Domain.Entities;
using Api.Catalog.Domain.Models;

namespace Api.Catalog.Domain.ValueObjects;

public record ProductSnapshot
{
    public Guid Id { get; init; }
    public string Description { get; init; }
    public string? Reference { get; init; }

    private ProductSnapshot(Guid id, string description, string? reference)
    {
        Id = id;
        Description = description;
        Reference = reference;
    }

    public static Result<ProductSnapshot> Create(Product product)
    {
        if (product is null)
            return DomainResultFailures.Validation("Informe um produto para a snapshot.");

        return new ProductSnapshot(product.Id, product.Description, product.Reference);
    }
};
