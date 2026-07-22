using Api.Catalog.Application.Contracts.Contexts;

namespace Api.Catalog.Api.Contexts;

public class HttpPersonContext(IHttpContextAccessor accessor) : IPersonContext
{
    public Guid? PersonId => Guid.TryParse(accessor.HttpContext?.User?.FindFirst("sub")?.Value, out var userId)
        ? userId
        : null;
}
