using Api.Catalog.Application.Entities.Platform;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal sealed class PlatformUserRoleMap : BaseMap<PlatformUserRole>
{
    public override void Configure(EntityTypeBuilder<PlatformUserRole> builder)
    {
        base.Configure(builder);
        builder.ToTable("platform_user_roles");

        builder.Property(p => p.UserId).HasColumnName("user_id");
        builder.Property(p => p.RoleId).HasColumnName("role_id");
    }
}
