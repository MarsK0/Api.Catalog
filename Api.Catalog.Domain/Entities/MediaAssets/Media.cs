namespace Api.Catalog.Domain.Entities;

public class Media : TenantScopedEntity
{
    public long Size { get; private set; }
    public string Extension { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    // Usar SHA256
    public byte[] Hash { get; private set; } = null!;

    private Media() { }

    public static AppResult<Media> Create(
        long size,
        string extension,
        string contentType,
        byte[] hash
    )
    {
        return new Media
        {
            Size = size,
            Extension = extension,
            ContentType = contentType,
            Hash = hash
        };
    }
}
