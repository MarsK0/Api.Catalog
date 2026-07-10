namespace Api.Catalog.Domain.Entities;

public class Product : TenantScopedEntity
{
    public string Description { get; private set; } = null!;
    public string? Reference { get; private set; }

    private Product() { }

    public static AppResult<Product> Create(
        string description,
        string? reference = null
    )
    {
        return new Product
        {
            Description = description,
            Reference = reference
        };
    }
}
