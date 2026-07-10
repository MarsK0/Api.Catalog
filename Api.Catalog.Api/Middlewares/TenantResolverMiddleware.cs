using Api.Catalog.Api.Constants;
using Api.Catalog.Api.Models;
using Api.Catalog.Domain;
using Api.Catalog.Infrastructure.Contracts;
using System.Net;
using System.Text.Json;

namespace Api.Catalog.Api.Middlewares;

public class TenantResolverMiddleware(
    RequestDelegate next,
    ILogger<TenantResolverMiddleware> logger
)
{
    public async Task InvokeAsync(HttpContext context, ITenantStore tenantStore)
    {
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await next(context);
            return;
        }

        await ResolveTenantId(context, tenantStore)
            .FoldAsync(
                onSuccess: async (resolved) =>
                {
                    context.Items[ConstantValues.TenantContextItemKey] = resolved;
                    await next(context);
                },
                onFailure: async (failure) =>
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new { failure.Message }));
                }
            );
    }

    // Busca ID do tenant ou slug como fallback no header. Se encontra slug, busca tenantId. Ambos os casos retorna tenantId ou falha.
    private async Task<AppResult<HttpTenantContextData>> ResolveTenantId(HttpContext context, ITenantStore tenantStore)
    {
        var headerValue = context.Request.Headers[ConstantValues.TenantHeaderName].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(headerValue))
        {
            logger.LogWarning("Header {Header} ausente na requisição", ConstantValues.TenantHeaderName);
            return AppFailure.InvalidRequest("Empresa não definida na requisição. Contate o suporte.");
        }

        if (
            headerValue.Equals(ConstantValues.PlatformSlug, StringComparison.OrdinalIgnoreCase) ||
            headerValue.Equals(ConstantValues.PlatformContextIdentifier, StringComparison.OrdinalIgnoreCase)
        )
            return new HttpTenantContextData(null, true);


        if (Guid.TryParse(headerValue, out var tenantId))
        {
            return await tenantStore.TenantExistsAsync(tenantId)
                ? new HttpTenantContextData(tenantId, false)
                : AppFailure.InvalidRequest("Empresa não encontrada. Contate o suporte.");
        }

        var id = await tenantStore.GetTenantIdBySlugAsync(headerValue);
        if (id.HasValue)
            return new HttpTenantContextData(id, false);

        return AppFailure.InvalidRequest("Empresa não encontrada. contate o suporte.");
    }
}
