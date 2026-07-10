using Api.Catalog.Domain.Entities;

namespace Api.Catalog.Application.Contracts;

public interface IPersonRepo
{
    Task CreateAsync(Person person, CancellationToken ct);
}
