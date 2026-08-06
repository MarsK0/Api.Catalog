using Api.Catalog.Api.Authorization;
using Api.Catalog.Api.Constants;
using Api.Catalog.Application;
using Api.Catalog.Application.Contracts.Contexts;
using Api.Catalog.Application.Models;
using Api.Catalog.Domain;
using Api.Catalog.Domain.Models;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Catalog.Api.Controllers;

[ApiController]
[Route("api/tenancy")]
public class TenancyController(IMediator mediator) : CatalogBaseController
{
    [HttpGet("slugexists")]
    [AllowAnonymous]
    public IActionResult SlugExists(ITenantContext tenantContext)
    {
        if (tenantContext.TenantId is not null || tenantContext.IsPlatformContext)
            return Ok();
        else
            return HandleFailure(DomainResultFailures.EntityNotFound("Empresa não encontrada para o Slug informado"));
    }
    [HttpGet("tenant")]
    [RequirePermission(PermissionValues.SystemPermissions.Tenants.Read)]
    public async Task<IActionResult> PaginatedList([FromQuery] GetTenantPaginatedListQuery query, CancellationToken ct)
    {
        return await mediator.Send(query, ct)
            .FoldAsync(
                (result) => Ok(result),
                HandleFailure
            );
    }
    [HttpGet("tenant/{id:guid}")]
    [RequirePermission(PermissionValues.SystemPermissions.Tenants.Read)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        return await mediator.Send(new GetTenantByIdQuery(id), ct)
            .FoldAsync(
                (result) => Ok(result),
                HandleFailure
            );
    }

    [HttpPost("tenant")]
    [RequirePermission(PermissionValues.SystemPermissions.Tenants.Create)]
    public async Task<IActionResult> Create([FromBody] CreateTenantCommand command, CancellationToken ct)
    {
        return await mediator.Send(command, ct)
            .FoldAsync(
                (result) => CreatedAtAction(
                    nameof(GetById),
                    new { id = result.Id },
                    result
                ),
                HandleFailure
            );
    }

    [HttpPatch("tenant/{id:guid}")]
    [RequirePermission(PermissionValues.SystemPermissions.Tenants.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTenantCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return HandleFailure(AppResultFailures.InvalidRequest("O id informado na requisição não condiz com o id da entidade."));

        return await mediator.Send(command, ct)
            .FoldAsync(
                (result) => Ok(result),
                HandleFailure
            );
    }

    [HttpDelete("tenant/{id:guid}")]
    [RequirePermission(PermissionValues.SystemPermissions.Tenants.Delete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        return await mediator.Send(new DeleteTenantCommand(id), ct)
            .FoldAsync(
                () => NoContent(),
                HandleFailure
            );
    }
}
