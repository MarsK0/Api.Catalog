using Api.Catalog.Application.Contracts.Contexts;

namespace Api.Catalog.Api.Contexts;

public class HttpPersonContext : IPersonContext
{
    private readonly Guid? _personId;
    public HttpPersonContext(IHttpContextAccessor accessor)
    {
        var id = accessor.HttpContext?.User?.FindFirst("sub")?.Value;
        _personId = Guid.TryParse(id, out var userId) ? userId : null; 
    }
    public Guid? PersonId => _personId;
}
