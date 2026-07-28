using Api.Catalog.Application.Contracts;
using Api.Catalog.Infrastructure.Contracts;
using Api.Catalog.Infrastructure.Persistence.Cache;
using Api.Catalog.Infrastructure.Persistence.PostgreSQL;
using Api.Catalog.Infrastructure.Persistence.PostgreSQL.Interceptors;
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
            (srvcs, options) =>
            {
                var tenantInterceptor = srvcs.GetRequiredService<TenantInterceptor>();
                var trackingInterceptor = srvcs.GetRequiredService<TrackingInterceptor>();
                var auditLogInterceptor = srvcs.GetRequiredService<AuditLogInterceptor>();
                options
                    .UseNpgsql(configuration.GetConnectionString("CatalogDb"))
                    .AddInterceptors(
                        tenantInterceptor,
                        trackingInterceptor,
                        auditLogInterceptor
                    );
            }
        );
        services.AddDbContextFactory<AuditDbContext>(
            options => options.UseNpgsql(configuration.GetConnectionString("CatalogDb")),
            lifetime: ServiceLifetime.Scoped
        );
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDataSeeder, PostgresSeed>();
        services.AddScoped<TenantInterceptor>();
        services.AddSingleton<TrackingInterceptor>();
        services.AddScoped<AuditLogInterceptor>();
        #endregion

        #region Repositories

        #region Application

        #region Authorization
        services.AddScoped<IAccountRepo, AccountRepo>();
        services.AddScoped<IRefreshTokenRepo, RefreshTokenRepo>();
        #endregion

        #region Platform
        services.AddScoped<IPlatformUserRepo, PlatformUserRepo>();
        #endregion

        #endregion

        #region Domain

        #region Tables
        services.AddScoped<IPersonRepo, PersonRepo>();
        #endregion

        #region Tenancy
        services.AddScoped<ITenantRepo, TenantRepo>();
        #endregion

        #endregion

        #endregion

        #region Services
        services.AddHybridCache();
        services.AddSingleton<ICacheService, AppCacheService>();
        services.AddScoped<ITenantStore, PostgresTenantStore>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPermissionValidator, PermissionValidatorService>();
        services.AddScoped<IModuleValidator, ModuleValidatorService>();
        services.AddSingleton<IPasswordHashService, BCryptPasswordHashService>();
        #endregion

        #region Providers
        services.AddSingleton<TimeProvider>(TimeProvider.System);
        #endregion

        return services;
    }
}
