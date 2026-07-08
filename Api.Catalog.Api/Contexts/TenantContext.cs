using Api.Catalog.Application.Contracts.Contexts;

namespace Api.Catalog.Api.Contexts;

public sealed class TenantContext : ITenantContext
{
    // Chave usada no Middleware: context.Items["tenant_id"]
    public const string TenantItemKey = "tenant_id";
    private readonly IHttpContextAccessor _accessor;

    public TenantContext(IHttpContextAccessor accessor, IConfiguration config)
    {
        _accessor = accessor;
    }
    public bool HasHttpContext => _accessor.HttpContext is not null;
    public Guid TenantId
    {
        get
        {
            if (!HasHttpContext)
                return Guid.Empty;

            var tenantItem = _accessor.HttpContext!.Items[TenantItemKey];
            if (tenantItem is Guid tenantId)
                return tenantId;

            if (tenantItem is not null && Guid.TryParse(tenantItem.ToString(), out var parsedTenantId))
                return parsedTenantId;

            throw new InvalidOperationException("Tenant ID não foi resolvido para a requisição.");
        }
    }
}
