using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class TenantMap : BaseMap<Tenant>
{
    public override void Configure(EntityTypeBuilder<Tenant> builder)
    {
        base.Configure(builder);
        builder.ToTable("tenant");

        builder.Property(p => p.Name).HasColumnName("name").HasMaxLength(60);
        builder.Property(p => p.Slug).HasColumnName("slug").HasMaxLength(30);

        builder.Navigation(n => n.Membership)
            .HasField("_membership")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(n => n.Modules)
            .HasField("_modules")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
