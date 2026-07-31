using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Mappers;
using Api.Catalog.Application.Models;
using Api.Catalog.Domain;
using Mediator;

namespace Api.Catalog.Application.Handlers;

internal sealed class UpdateTenantHandler(
    IUnitOfWork unitOfWork,
    ITenantRepo tenantRepo
) : IRequestHandler<UpdateTenantCommand, AppResult<TenantDto>>
{
    public async ValueTask<AppResult<TenantDto>> Handle(UpdateTenantCommand command, CancellationToken ct)
    {
        var tenant = await tenantRepo.GetByIdAsync(command.TenantId, ct, track: true);
        if (tenant is null)
            return AppFailure.EntityNotFound("Não foi encontrado um tenant para o id informado.");

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
