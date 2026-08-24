using Api.Catalog.Domain.Models;

namespace Api.Catalog.Domain.Entities;

public class Media : TenantScopedEntity
{
    public long Size { get; private set; }
    public string Extension { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    // Usar SHA256
    public byte[] Hash { get; private set; } = null!;

    private Media() { }

    public static Result<Media> Create(
        long size,
        string extension,
        string contentType,
        byte[] hash
    )
    {
        if (size <= 0)
            return DomainResultFailures.Validation("O tamanho da mídia deve ser superior a zero.");

        if (string.IsNullOrWhiteSpace(extension))
            return DomainResultFailures.Validation("Uma extensão deve ser informado para a mídia.");

        if (string.IsNullOrWhiteSpace(contentType))
            return DomainResultFailures.Validation("Um tipo de conteúdo deve ser informado para a mídia");

        if (hash.Length == 0)
            return DomainResultFailures.Validation("A hash da mídia deve ser informada");

        return new Media
        {
            Size = size,
            Extension = extension,
            ContentType = contentType,
            Hash = hash
        };
    }
}
