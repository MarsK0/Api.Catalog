using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL.Context;

internal sealed class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var apiDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Api.Catalog.Api"));

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var config = new ConfigurationBuilder()
            .SetBasePath(apiDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: false)
            .Build();

        var connectionString = config.GetConnectionString("CatalogDb")
            ?? throw new InvalidOperationException($"String de conexão PostgreSQL não configurada. Verifique o arquivo appsettings.{environment}.json.");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDbContext(options, new TenantEmptyContext());
    }
}
