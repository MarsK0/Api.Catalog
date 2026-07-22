using Api.Catalog.Api.Constants;
using Api.Catalog.Api.Models;
using Api.Catalog.Application.Contracts.Contexts;

namespace Api.Catalog.Api.Contexts;
/// <summary>
/// Implementação de ITenantContext baseada no ciclo de vida HTTP.
/// 
/// Nota arquitetural:
/// O tenant atualmente é resolvido única e exclusivamente via IHttpContextAccessor.
/// Isso acopla (mesmo que fracamente) esta implementação ao ASP.NET Core HTTP pipeline.
/// A alternativa considerada é uma resolução explícita de contexto
/// via composição de DI (ex.: startup scope, request scope, worker scope...).
/// 
/// Não mover IHttpContextAccessor para consumidores de ITenantContext.
/// </summary>
public sealed class HttpTenantContext(IHttpContextAccessor accessor) : ITenantContext
{
    private HttpTenantContextData _context => (
            accessor.HttpContext?.Items.TryGetValue(ConstantValues.TenantContextItemKey, out var value) is true &&
            value is HttpTenantContextData context
        )
        ? context
        : new HttpTenantContextData(null, false);
    public Guid? TenantId => _context.TenantId;
    public bool IsPlatformContext => _context.IsPlatformContext;
}
