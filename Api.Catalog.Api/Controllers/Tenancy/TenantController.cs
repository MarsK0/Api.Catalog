using Api.Catalog.Api.Authorization;
using Api.Catalog.Api.Constants;
using Api.Catalog.Application.Contracts.Contexts;
using Api.Catalog.Application.Models;
using Api.Catalog.Domain;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Catalog.Api.Controllers.Tenancy;

[ApiController]
[Route("api/tenancy")]
public class TenantController(IMediator mediator) : CatalogBaseController
{
    [HttpGet("slugexists")]
    [AllowAnonymous]
    public IActionResult SlugExists(ITenantContext tenantContext, CancellationToken ct)
    {
        if (tenantContext.TenantId is not null || tenantContext.IsPlatformContext)
            return Ok();
        else
            return HandleFailure(AppFailure.EntityNotFound("Empresa não encontrada para o Slug informado"));
    }
    [HttpGet("tenant/{id:guid}")]
    [RequirePermission(Permissions.PlatformPermissions.Tenants.Read)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        return await mediator.Send(new GetTenantByIdQuery(id), ct)
            .FoldAsync(
                (result) => Ok(result),
                HandleFailure
            );
    }

    [HttpPost("tenant")]
    [RequirePermission(Permissions.PlatformPermissions.Tenants.Create)]
    public async Task<IActionResult> Create([FromBody] CreateTenantCommand command, CancellationToken ct)
    {
        return await mediator.Send(command, ct)
            .FoldAsync(
                (result) => CreatedAtAction(
                    nameof(GetById),
                    new { id = result.TenantId },
                    result
                ),
                HandleFailure
            );
    }
}
