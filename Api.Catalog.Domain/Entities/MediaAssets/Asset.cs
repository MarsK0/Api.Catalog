using Api.Catalog.Domain.Models;
using System.Diagnostics.CodeAnalysis;

namespace Api.Catalog.Domain.Entities;

public class Asset : TenantScopedEntity
{
    public Guid MediaId { get; private set; }
    private readonly Media _media = null!;
    public Media Media => _media;
    public string FileName { get; private set; } = null!;

    private Asset() { }

    public static Result<Asset> Create(
        Guid mediaId,
        string fileName
    )
    {
        if (mediaId == Guid.Empty)
            return DomainResultFailures.Validation("Uma mídia deve ser informada para o recurso.");

        if (string.IsNullOrWhiteSpace(fileName))
            return DomainResultFailures.Validation("Um nome de arquivo deve ser informado para o recurso.");

        return new Asset
        {
            MediaId = mediaId,
            FileName = fileName
        };
    }
}
