using Api.Catalog.Application;
using Api.Catalog.Domain;
using Api.Catalog.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Catalog.Api.Controllers;

public abstract class CatalogBaseController : ControllerBase
{
    protected IActionResult HandleFailure(Failure failure)
    {
        return failure.Code switch
        {
            DomainFailureCodes.Validation => BadRequest(new { failure.Message }),
            DomainFailureCodes.EntityNotFound => NotFound(new { failure.Message }),
            AppFailureCodes.Validation => BadRequest(new { failure.Message }),
            AppFailureCodes.Unauthorized => Unauthorized(new { failure.Message }),
            AppFailureCodes.InvalidRequest => BadRequest(new { failure.Message }),
            _ => throw new Exception($"FailureCode '{failure.Code}' não mapeado em {nameof(HandleFailure)}.")
        };
    }
}
