using Api.Catalog.Api.Constants;
using Api.Catalog.Application.Contracts.Contexts;

namespace Api.Catalog.Api.Contexts;

public sealed class TenantContext(IHttpContextAccessor accessor) : ITenantContext
{
    private readonly bool _isPlatformContext = accessor.HttpContext!.Items[ConstantValues.TenantContextItemKey]?.ToString() == ConstantValues.PlatformContextIdentifier;

    public bool IsPlatformContext => _isPlatformContext;
    public bool HasHttpContext => accessor.HttpContext is not null;
    public Guid TenantId
    {
        get
        {
            if (!HasHttpContext || IsPlatformContext)
                return Guid.Empty;

            var _tenantItem = accessor.HttpContext!.Items[ConstantValues.TenantContextItemKey];
            if (_tenantItem is Guid tenantId)
                return tenantId;

            if (_tenantItem is not null && Guid.TryParse(_tenantItem.ToString(), out var parsedTenantId))
                return parsedTenantId;

            throw new InvalidOperationException("Tenant ID não foi resolvido para a requisição.");
        }
    }
}
