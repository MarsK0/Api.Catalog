using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class PersonTenantRoleMap : TenantScopedMap<PersonTenantRole>
{
    public override void Configure(EntityTypeBuilder<PersonTenantRole> builder)
    {
        base.Configure(builder);
        builder.ToTable("person_tenant_roles");

        builder.Property(p => p.PersonId).HasColumnName("person_id");
        builder.Property(p => p.TenantRoleId).HasColumnName("tenant_role_id");
    }
}
