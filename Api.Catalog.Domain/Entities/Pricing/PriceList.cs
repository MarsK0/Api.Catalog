namespace Api.Catalog.Domain.Entities;

public class PriceList : TenantScopedEntity
{
    public string Name { get; set; } = null!;
    public DateTimeOffset? ValidFrom { get; set; }
    public DateTimeOffset? ValidUntil { get; set; }

    private PriceList() { }

    public static AppResult<PriceList> Create(
        string Name,
        DateTimeOffset? ValidFrom,
        DateTimeOffset? ValidUntil
    )
    {
        return new PriceList
        {
            Name = Name,
            ValidFrom = ValidFrom,
            ValidUntil = ValidUntil
        };
    }

}
