using Api.Catalog.Domain;
using Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Catalog.Application.Models;

public record DeleteTenantCommand(Guid id) : IRequest<AppResult>;