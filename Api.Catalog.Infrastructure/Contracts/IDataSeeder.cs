namespace Api.Catalog.Infrastructure.Contracts;

public interface IDataSeeder
{
    Task SeedAsync(CancellationToken ct);
}
