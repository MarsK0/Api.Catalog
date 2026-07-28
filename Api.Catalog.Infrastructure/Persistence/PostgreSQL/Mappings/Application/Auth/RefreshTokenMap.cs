using Api.Catalog.Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class RefreshTokenMap : BaseMap<RefreshToken>
{
    public override void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        base.Configure(builder);
        builder.ToTable("refresh_token");

        builder.Property(p => p.UserId).HasColumnName("user_id");
        builder.Property(p => p.TokenHash).HasColumnName("token_hash");
        builder.Property(p => p.FamilyId).HasColumnName("family_id");
        builder.Property(p => p.Expires).HasColumnName("expires");
        builder.Property(p => p.RememberMe).HasColumnName("remember_me");
        builder.Property(p => p.Revoked).HasColumnName("revoked");
        builder.Property(p => p.IsUsed).HasColumnName("is_used");

        builder.HasIndex(i => i.TokenHash)
            .IsUnique();
    }
}
