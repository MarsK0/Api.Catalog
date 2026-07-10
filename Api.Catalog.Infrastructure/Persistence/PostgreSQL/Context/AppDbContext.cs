using Api.Catalog.Application.Contracts.Contexts;
using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

public sealed class AppDbContext : DbContext
{
    private readonly ITenantContext? _tenantContext;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

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
        modelBuilder.HasDefaultSchema("catalog");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Percorre os tipos das entidades, valida se é do tipo TenantScopedEntity, cria genericamente
        // o método ApplyTenantFilter e o invoca com modelBuilder como argumento
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(TenantScopedEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(AppDbContext)
                    .GetMethod(nameof(ApplyTenantFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(this, new object[] { modelBuilder });
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    private void ApplyTenantFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : TenantScopedEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(
            entity => (
                entity.TenantId == _tenantContext!.TenantId
            )
        );
    }
    public override int SaveChanges()
    {
        ApplyTenantIdInNewEntities();
        return base.SaveChanges();
    }
    public override Task<int> SaveChangesAsync(CancellationToken ct)
    {
        ApplyTenantIdInNewEntities();
        return base.SaveChangesAsync();
    }
    private void ApplyTenantIdInNewEntities()
    {
        foreach (var entry in ChangeTracker.Entries<TenantScopedEntity>())
            if (entry.State == EntityState.Added && entry.Entity.TenantId == Guid.Empty)
                entry.Entity.SetTenant(_tenantContext!.TenantId);
    }
}