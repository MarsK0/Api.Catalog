using Api.Catalog.Application.Contracts.Contexts;

namespace Api.Catalog.Api.Contexts;

public class HttpUserContext(IHttpContextAccessor accessor) : IUserContext
{
    public Guid? UserId => Guid.TryParse(accessor.HttpContext?.User?.FindFirst("sub")?.Value, out var userId)
        ? userId
        : null;
}
