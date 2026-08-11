using Api.Catalog.Domain.Entities;

namespace Api.Catalog.Application.Contracts;

public interface IRoleRepo
{
    void Add(Role role);
}