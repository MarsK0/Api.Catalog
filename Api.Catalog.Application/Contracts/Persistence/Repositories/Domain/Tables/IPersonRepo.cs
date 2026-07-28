using Api.Catalog.Domain.Entities;

namespace Api.Catalog.Application.Contracts;

public interface IPersonRepo
{
    void Add(Person person);
}
