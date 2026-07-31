using Api.Catalog.Application.Models;
using Api.Catalog.Domain;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Catalog.Api.Controllers;

[ApiController]
[Route("api/modules")]
public class ModuleController(IMediator mediator) : CatalogBaseController
{
    [HttpGet]
    [AllowAnonymous]
    public Task<IActionResult> List(CancellationToken ct)
    {
        return mediator.Send(new GetModulesQuery(), ct)
            .FoldAsync(
                (result) => Ok(result),
                HandleFailure
            );
    }
}
