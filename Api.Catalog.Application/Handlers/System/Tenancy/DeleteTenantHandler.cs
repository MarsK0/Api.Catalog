using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Models;
using Api.Catalog.Domain;
using Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Catalog.Application.Handlers;

internal sealed class DeleteTenantHandler(
    IUnitOfWork unitOfWork,
    ITenantRepo tenantRepo
) : IRequestHandler<DeleteTenantCommand, AppResult>
{
    public async ValueTask<AppResult> Handle(DeleteTenantCommand command, CancellationToken ct)
    {
        await tenantRepo.DeleteByIdAsync(command.id, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return AppResult.Success;
    }
}
