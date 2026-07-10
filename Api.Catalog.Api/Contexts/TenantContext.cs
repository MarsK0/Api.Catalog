using Api.Catalog.Api.Constants;
using Api.Catalog.Api.Models;
using Api.Catalog.Application.Contracts.Contexts;

namespace Api.Catalog.Api.Contexts;

public sealed class TenantContext : ITenantContext
{
    private readonly TenantHttpContextData _context;
    public TenantContext(IHttpContextAccessor accessor)
    {
        if (
            accessor.HttpContext?.Items.TryGetValue(ConstantValues.TenantContextItemKey, out var value) is true &&
            value is TenantHttpContextData context
        )
        {
            _context = context;
        }
        else
        {
            // Empty context
            _context = new TenantHttpContextData(null, false);
        }
    }
    public Guid? TenantId => _context.TenantId;
    public bool IsPlatformContext => _context.IsPlatformContext;
}
