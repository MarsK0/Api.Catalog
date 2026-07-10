using Api.Catalog.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Api.Catalog.Api.Models;

public class AppBaseController : ControllerBase
{
    protected IActionResult HandleFailure(AppFailure failure)
    {
        return failure.Code switch
        {
            FailureCode.DomainValidation => BadRequest(new { failure.Message }),
            FailureCode.ApplicationValidation => BadRequest(new { failure.Message }),
            FailureCode.InfrastructureValidation => BadRequest(new { failure.Message }),
            FailureCode.EntityNotFound => NotFound(new { failure.Message }),
            _ => throw new Exception(failure?.Message ?? "Falha indefinida.")
        };
    }
}
