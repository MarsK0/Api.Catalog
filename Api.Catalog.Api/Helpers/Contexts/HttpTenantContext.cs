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
public class HttpTenantContext(IHttpContextAccessor accessor) : ITenantContext
{
    protected HttpTenantContextData Context
    {
        get => accessor.HttpContext?.Items.TryGetValue(ConstantValues.TenantContextItemKey, out var value) is true &&
               value is HttpTenantContextData context
            ? context
            : new HttpTenantContextData(null, false);
        set
        {
            var httpContext = accessor.HttpContext
                ?? throw new InvalidOperationException(
                    "Não é possível definir o tenant fora do ciclo de vida de uma requisição HTTP.");
            httpContext.Items[ConstantValues.TenantContextItemKey] = value;
        }
    }
    public Guid? TenantId => Context.TenantId;
    public bool IsPlatformContext => Context.IsPlatformContext;
    public bool AllowCrossTenancy => Context.IsPlatformContext && Context.TenantId is null;
}

public sealed class HttpMutableTenantContext : HttpTenantContext, IMutableTenantContext
{
    public HttpMutableTenantContext(IHttpContextAccessor accessor) : base(accessor)
    {
        if (!IsPlatformContext)
            throw new InvalidOperationException("Não é permitido utilizar um contexto de tenant mutável fora de um contexto de plataforma");
    }

    public void SetTenant(Guid? tenantId)
        => Context = Context with { TenantId = tenantId };
}