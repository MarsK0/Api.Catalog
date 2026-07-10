using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class MediaMap : TenantScopedMap<Media>
{
    public override void Configure(EntityTypeBuilder<Media> builder)
    {
        base.Configure(builder);
        builder.ToTable("media");

        builder.Property(p => p.Size).HasColumnName("size");
        builder.Property(p => p.Extension).HasColumnName("extension").HasMaxLength(10);
        builder.Property(p => p.ContentType).HasColumnName("content_type").HasMaxLength(100);
        builder.Property(p => p.Hash).HasColumnName("hash").HasColumnType("bytea");
    }
}
