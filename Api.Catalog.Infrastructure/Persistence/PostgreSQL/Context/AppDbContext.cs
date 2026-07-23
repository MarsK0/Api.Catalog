using Api.Catalog.Application.Contracts.Contexts;
using Api.Catalog.Application.Entities;
using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options,
    ITenantContext tenantContext
) : DbContext(options)
{
    #region Application
    public DbSet<PlatformMembership> PlatformMembership { get; init; }
    public DbSet<Account> Accounts { get; init; }
    public DbSet<RefreshToken> RefreshTokens { get; init; }
    #endregion
    #region Domain
    #region #Budget
    public DbSet<Budget> Budgets { get; init; }
    public DbSet<BudgetItem> BudgetItems { get; init; }
    #endregion
    #region Catalog
    public DbSet<Category> Categories { get; init; }
    #endregion
    #region Media
    public DbSet<Asset> Assets { get; init; }
    public DbSet<Media> Media { get; init; }
    #endregion
    #region Identity
    public DbSet<TenantRole> TenantRoles { get; init; }
    public DbSet<PlatformRole> PlatformRoles { get; init; }
    #endregion
    #region Pricing
    public DbSet<PriceList> PriceLists { get; init; }
    #region Price Rules
    public DbSet<DirectPriceRule> DirectPricesRules { get; init; }
    public DbSet<QuantityPriceRule> QuantityPricesRules { get; init; }
    #endregion
    #endregion
    #region Tables
    public DbSet<Product> Products { get; init; }
    public DbSet<Person> Persons { get; init; }
    #endregion
    #region Tenancy
    public DbSet<Tenant> Tenants { get; init; }
    public DbSet<TenantModule> TenantModules { get; init; }
    public DbSet<TenantMembership> TenantMembership { get; init; }
    #endregion
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("catalog");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        modelBuilder.ApplyDataRules(tenantContext);
    }
}