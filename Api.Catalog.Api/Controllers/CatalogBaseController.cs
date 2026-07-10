using Api.Catalog.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Api.Catalog.Api.Controllers;

public abstract class CatalogBaseController : ControllerBase
{
    protected IActionResult HandleFailure(AppFailure failure)
    {
        return failure.Code switch
        {
            FailureCode.DomainValidation => BadRequest(new { failure.Message }),
            FailureCode.ApplicationValidation => BadRequest(new { failure.Message }),
            FailureCode.InfrastructureValidation => BadRequest(new { failure.Message }),
            FailureCode.Unauthorized => Unauthorized(new { failure.Message }),
            FailureCode.Conflict => Conflict(new { failure.Message }),
            FailureCode.InvalidRequest => BadRequest(new { failure.Message }),
            FailureCode.EntityNotFound => NotFound(new { failure.Message }),
            _ => throw new Exception("Falha indefinida")
        };
    }
}
