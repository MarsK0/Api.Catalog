using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class PersonRoleMap : TenantScopedMap<PersonRole>
{
    public override void Configure(EntityTypeBuilder<PersonRole> builder)
    {
        base.Configure(builder);
        builder.ToTable("person_roles");

        builder.Property(p => p.PersonId).HasColumnName("person_id");
        builder.Property(p => p.RoleId).HasColumnName("role_id");
    }
}
