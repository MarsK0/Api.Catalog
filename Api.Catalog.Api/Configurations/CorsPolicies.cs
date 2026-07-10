namespace Api.Catalog.Api.Configurations;

public static class CorsPolicies
{
    public const string DefaultPolicy = "CatalogCorsPolicy";

    public static IServiceCollection AddCorsPolicies(
        this IServiceCollection services,
        IConfiguration config,
        IWebHostEnvironment environment
    )
    {
        var origins = config.GetSection("Cors:Origins")?.Get<string[]>() ?? [];
        if (!environment.IsDevelopment() && !origins.Any())
            throw new InvalidOperationException("Cors:Origins deve ser configurado fora de ambiente de desenvolvimento.");

        if (origins.Any(o => o.Equals("*")))
            throw new InvalidOperationException("Proibido o uso de wildcard '*' nas configurações de Cors. Defina origens explícitas.");

        services.AddCors(options =>
        {
            options.AddPolicy(DefaultPolicy, policy =>
            {
                policy
                    .WithOrigins(origins)
                    .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
                    .WithHeaders("Content-Type", "Authorization", "X-Requested-With")
                    .AllowCredentials()
                    .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
            });
        });

        return services;
    }
}