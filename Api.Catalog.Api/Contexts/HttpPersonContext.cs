using Api.Catalog.Api.Models;
using Api.Catalog.Application.Contracts.Contexts;
using System.IdentityModel.Tokens.Jwt;

namespace Api.Catalog.Api.Contexts;

public class HttpPersonContext : IPersonContext
{
    private readonly HttpPersonContextData _context;

    public HttpPersonContext(IHttpContextAccessor accessor)
    {
        if (
            accessor.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value is string sub &&
            Guid.TryParse(sub, out Guid personId)
        )
        {
            _context = new HttpPersonContextData(personId);
        }
        else
        {
            // Empty context
            _context = new HttpPersonContextData(null);
        }
    }

    public Guid? PersonId => _context.PersonId;
}
