using Api.Catalog.Application.Contracts;
using Api.Catalog.Domain.Entities;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal sealed class PersonRepo(
    AppDbContext db
) : IPersonRepo
{
    public void Add(Person person) => db.Persons.Add(person);
}
