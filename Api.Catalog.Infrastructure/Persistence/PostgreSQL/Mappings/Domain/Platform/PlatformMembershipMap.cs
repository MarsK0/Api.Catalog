using Api.Catalog.Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class PlatformMembershipMap : BaseMap<PlatformMembership>
{
    public override void Configure(EntityTypeBuilder<PlatformMembership> builder)
    {
        base.Configure(builder);
        builder.ToTable("platform_membership");

        builder.Property(p => p.PersonId).HasColumnName("person_id");
        builder.HasOne(o => o.Person)
            .WithOne()
            .HasForeignKey<PlatformMembership>(fk => fk.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(n => n.Person)
            .HasField("_person")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

