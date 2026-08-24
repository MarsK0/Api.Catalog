using Api.Catalog.Domain.Models;

namespace Api.Catalog.Domain.Entities;

public class PriceList : TenantScopedEntity
{
    public string Name { get; set; } = null!;
    public DateTimeOffset? ValidFrom { get; set; }
    public DateTimeOffset? ValidUntil { get; set; }

    private PriceList() { }

    public static Result<PriceList> Create(
        string name,
        DateTimeOffset? validFrom,
        DateTimeOffset? validUntil
    )
    {
        if (string.IsNullOrWhiteSpace(name))
            return DomainResultFailures.Validation("Um nome deve ser informado para a lista de preços.");

        return new PriceList
        {
            Name = name,
            ValidFrom = validFrom,
            ValidUntil = validUntil
        };
    }

}
