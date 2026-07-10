using Api.Catalog.Api.Constants;
using Api.Catalog.Api.Models;
using Api.Catalog.Application.Contracts.Contexts;

namespace Api.Catalog.Api.Contexts;

public sealed class TenantContext : ITenantContext
{
    private readonly TenantRequestContext _context;
    public TenantContext(IHttpContextAccessor accessor)
    {
        if(
            accessor.HttpContext?.Items.TryGetValue(ConstantValues.TenantContextItemKey, out var value) == true &&
            value is TenantRequestContext context
        )
        {
            _context = context;
        }
        else
        {
            _context = TenantRequestContext.Empty;
        }
    }
    public bool IsPlatformContext => _context.IsPlatformContext;
    public bool HasContext => _context.TenantId is not null || _context.IsPlatformContext;
    public Guid TenantId => _context.TenantId ?? throw new InvalidOperationException("Tenant ID não foi resolvido para a requisição ou acessado em um contexto inválido.");
}
