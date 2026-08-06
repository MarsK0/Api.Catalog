using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Mappers;
using Api.Catalog.Application.Models;
using Api.Catalog.Domain;
using Api.Catalog.Domain.Models;
using Mediator;

namespace Api.Catalog.Application.Handlers;

internal sealed class UpdateTenantHandler(
    IUnitOfWork unitOfWork,
    ITenantRepo tenantRepo
) : IRequestHandler<UpdateTenantCommand, Result<TenantDto>>
{
    public async ValueTask<Result<TenantDto>> Handle(UpdateTenantCommand command, CancellationToken ct)
    {
        var tenant = await tenantRepo.GetByIdAsync(command.Id, ct, track: true);
        if (tenant is null)
            return DomainResultFailures.EntityNotFound("Não foi encontrado um tenant para o id informado.");

        if (command.Name.IsSet)
        {
            var result = tenant.UpdateName(command.Name.Value);
            if (!result.IsSuccess)
                return result.Failure;
        }

        if (command.Modules.IsSet)
        {
            var result = tenant.UnlockModules(command.Modules.Value);
            if (!result.IsSuccess)
                return result.Failure;
        }

        await unitOfWork.SaveChangesAsync(ct);
        return tenant.Dto();
    }
}
