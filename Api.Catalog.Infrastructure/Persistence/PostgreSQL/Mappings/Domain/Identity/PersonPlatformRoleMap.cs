using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class PersonPlatformRoleMap : BaseMap<PersonPlatformRole>
{
    public override void Configure(EntityTypeBuilder<PersonPlatformRole> builder)
    {
        base.Configure(builder);
        builder.ToTable("person_platform_roles");

        builder.Property(p => p.PersonId).HasColumnName("person_id");
        builder.Property(p => p.PlatformRoleId).HasColumnName("platform_role_id");
    }
}
