using Api.Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Catalog.Application.Contracts;

public interface IRoleRepo
{
    void Add(Role role);
}