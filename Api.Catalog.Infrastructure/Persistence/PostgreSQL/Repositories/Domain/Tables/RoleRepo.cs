using Api.Catalog.Application.Contracts;
using Api.Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal sealed class RoleRepo(AppDbContext db) : IRoleRepo
{
    public void Add(Role role) => db.TenantRoles.Add(role);
}
