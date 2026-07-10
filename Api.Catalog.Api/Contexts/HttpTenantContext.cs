using Api.Catalog.Api.Constants;
using Api.Catalog.Api.Models;
using Api.Catalog.Application.Contracts.Contexts;

namespace Api.Catalog.Api.Contexts;

public sealed class HttpTenantContext : ITenantContext
{
    private readonly HttpTenantContextData _context;
    public HttpTenantContext(IHttpContextAccessor accessor)
    {
        if (
            accessor.HttpContext?.Items.TryGetValue(ConstantValues.TenantContextItemKey, out var value) is true &&
            value is HttpTenantContextData context
        )
        {
            _context = context;
        }
        else
        {
            // Empty context
            _context = new HttpTenantContextData(null, false);
        }
    }
    public Guid? TenantId => _context.TenantId;
    public bool IsPlatformContext => _context.IsPlatformContext;
}
