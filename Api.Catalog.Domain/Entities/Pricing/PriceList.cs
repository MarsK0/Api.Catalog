namespace Api.Catalog.Domain.Entities;

public class PriceList : TenantScopedEntity
{
    public string Name { get; set; } = null!;
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }

    private PriceList() { }

    public static AppResult<PriceList> Create(
        string Name,
        DateTime? ValidFrom,
        DateTime? ValidUntil
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
