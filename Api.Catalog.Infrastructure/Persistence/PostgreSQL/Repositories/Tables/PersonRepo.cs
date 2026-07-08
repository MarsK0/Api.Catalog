using Api.Catalog.Application.Contracts;
using Api.Catalog.Domain.Entities;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal sealed class PersonRepo(
    AppDbContext db
) : IPersonRepo
{
    public async Task CreateAsync(Person person, CancellationToken ct)
    {
        await db.Persons.AddAsync(person, ct);
    }
}
