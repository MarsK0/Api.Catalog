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

    public static Result<PersonSnapshot> Create(
        Guid id,
        string name
    )
    {
        if (id == Guid.Empty)
            return DomainResultFailures.Validation("Um id de pessoa deve ser informada para a snapshot da pessoa.");

        if (string.IsNullOrWhiteSpace(name))
            return DomainResultFailures.Validation("O nome da pessoa deve ser informado na snapshot.");

        return new PersonSnapshot(id, name);
    }
};