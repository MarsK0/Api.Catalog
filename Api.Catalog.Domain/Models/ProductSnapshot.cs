namespace Api.Catalog.Domain.Models;

public record ProductSnapshot
{
    public string Description { get; init; }
    public string? Reference { get; init; }

    private ProductSnapshot(string description, string? reference)
    {
        Description = description;
        Reference = reference;
    }

    public static Result<ProductSnapshot> Create(string description, string? reference)
    {
        if (string.IsNullOrWhiteSpace(description))
            return DomainResultFailures.Validation("Uma descrição deve ser informada para a snapshot do produto.");

        return new ProductSnapshot(description, string.IsNullOrWhiteSpace(reference) ? null : reference);
    }
};
