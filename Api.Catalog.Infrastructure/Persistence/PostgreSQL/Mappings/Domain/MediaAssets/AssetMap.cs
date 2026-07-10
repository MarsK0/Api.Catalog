using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class AssetMap : TenantScopedMap<Asset>
{
    public override void Configure(EntityTypeBuilder<Asset> builder)
    {
        base.Configure(builder);
        builder.ToTable("assets");

        builder.Property(p => p.MediaId).HasColumnName("media_id");
        builder.Property(p => p.FileName).HasColumnName("file_name").HasMaxLength(255);

        builder.HasOne(o => o.Media)
            .WithMany()
            .HasForeignKey(fk => fk.MediaId);
        builder.Navigation(n => n.Media)
            .HasField("_media")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
