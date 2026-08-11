using Api.Catalog.Application.Contracts;
using Api.Catalog.Domain.Entities;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal sealed class RoleRepo(AppDbContext db) : IRoleRepo
{
    public void Add(Role role) => db.TenantRoles.Add(role);
}
