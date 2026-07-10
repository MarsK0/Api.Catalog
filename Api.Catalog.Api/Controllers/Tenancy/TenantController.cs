using Api.Catalog.Api.Authorization;
using Api.Catalog.Application.Models;
using Api.Catalog.Domain;
using Api.Catalog.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Catalog.Api.Controllers.Tenancy;

[ApiController]
[Route("api/tenancy")]
[RequirePermission(AppPermissions.PlatformPermissions.Tenants.Base)]
public class TenantController(IMediator mediator) : CatalogBaseController
{
    [HttpGet("tenant/{id:guid}")]
    [RequirePermission(AppPermissions.PlatformPermissions.Tenants.Read)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        return await mediator.Send(new GetTenantByIdQuery(id), ct)
            .FoldAsync(
                (result) => Ok(result),
                HandleFailure
            );
    }
    [HttpPost("tenant")]
    [RequirePermission(AppPermissions.PlatformPermissions.Tenants.Create)]
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
