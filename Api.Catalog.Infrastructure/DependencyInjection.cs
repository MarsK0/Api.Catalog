using Api.Catalog.Application.Contracts;
using Api.Catalog.Infrastructure.Contracts;
using Api.Catalog.Infrastructure.Persistence.Cache;
using Api.Catalog.Infrastructure.Persistence.PostgreSQL;
using Api.Catalog.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        #region Database
        services.AddDbContext<AppDbContext>(
            options => options.UseNpgsql(configuration.GetConnectionString("CatalogDb"))
        );
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        #endregion

        #region Repositories
        #region Authorization
        services.AddScoped<IAccountRepo, AccountRepo>();
        services.AddScoped<IRefreshTokenRepo, RefreshTokenRepo>();
        #endregion
        #region Tenancy
        services.AddScoped<ITenantRepo, TenantRepo>();
        #endregion
        #endregion

        #region Services
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, AppCacheService>();
        services.AddScoped<ITenantStore, PostgresTenantStore>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddSingleton<IPasswordHashService, BCryptPasswordHashService>();
        #endregion

        #region Providers
        services.AddSingleton<TimeProvider>(TimeProvider.System);
        #endregion

        return services;
    }
}
