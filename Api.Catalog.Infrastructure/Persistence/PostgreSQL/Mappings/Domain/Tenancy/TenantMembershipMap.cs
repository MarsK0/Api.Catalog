using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class TenantMembershipMap : TenantScopedMap<TenantMembership>
{
    public override void Configure(EntityTypeBuilder<TenantMembership> builder)
    {
        base.Configure(builder);
        builder.ToTable("tenant_membership");

        builder.Property(p => p.PersonId).HasColumnName("person_id");

        builder.HasIndex(p => new { p.PersonId, p.TenantId })
            .IsUnique();

        builder.HasOne(m => m.Tenant)
            .WithMany(t => t.Membership)
            .HasForeignKey(fk => fk.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
