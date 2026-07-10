using System.Diagnostics.CodeAnalysis;

namespace Api.Catalog.Domain.Entities;

public class Asset : TenantScopedEntity
{
    public Guid MediaId { get; private set; }
    [SuppressMessage("Compiler", "CS0649", Justification = "Populado na camada de infra")]
    private Media _media = null!;
    public Media Media => _media;
    public string FileName { get; private set; } = null!;

    private Asset() { }

    private AppResult<Asset> Create(
        Guid mediaId,
        string fileName
    )
    {
        return new Asset
        {
            MediaId = mediaId,
            FileName = fileName
        };
    }
}
