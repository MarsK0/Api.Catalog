using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Models;
using Api.Catalog.Domain;
using Api.Catalog.Domain.Models;
using Mediator;

namespace Api.Catalog.Application.Handlers;

internal sealed class DeleteTenantHandler(
    IUnitOfWork unitOfWork,
    ITenantRepo tenantRepo
) : IRequestHandler<DeleteTenantCommand, Result>
{
    public async ValueTask<Result> Handle(DeleteTenantCommand command, CancellationToken ct)
    {
        var tenant = await tenantRepo.GetByIdAsync(command.TenantId, ct);
        if (tenant is null)
            return DomainResultFailures.EntityNotFound("Tenant não encontrado ou já excluído.");

        tenantRepo.Delete(tenant);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success;
    }
}
