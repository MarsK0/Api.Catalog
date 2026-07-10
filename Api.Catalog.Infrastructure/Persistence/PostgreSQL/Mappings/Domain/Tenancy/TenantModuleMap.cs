using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class TenantModuleMap : TenantScopedMap<TenantModule>
{
    public override void Configure(EntityTypeBuilder<TenantModule> builder)
    {
        base.Configure(builder);
        builder.ToTable("tenant_module");

        builder.Property(p => p.ModuleCode).HasColumnName("module_code");

        builder.HasIndex(p => new { p.ModuleCode, p.TenantId })
            .IsUnique();

        builder.HasOne(m => m.Tenant)
            .WithMany(t => t.Modules)
            .HasForeignKey(fk => fk.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
