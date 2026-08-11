using Api.Catalog.Domain.Entities;

namespace Api.Catalog.Domain.Models;

public record PersonSnapshot
{
    public Guid Id { get; init; }
    public string Name { get; init; }

    private PersonSnapshot(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public static Result<PersonSnapshot> Create(Person person)
    {
        if (person is null)
            return DomainResultFailures.Validation("Informe uma pessoa para a snapshot.");

        return new PersonSnapshot(person.Id, person.Name);
    }
};