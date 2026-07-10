using Api.Catalog.Domain;
using Api.Catalog.Infrastructure.Contracts;
using System.Net;
using System.Text.Json;

namespace Api.Catalog.Api.Middlewares;

public class TenantResolverMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantResolverMiddleware> _logger;

    private const string TenantHeaderName = "Catalog-Tenant";

    public TenantResolverMiddleware(
        RequestDelegate next,
        ILogger<TenantResolverMiddleware> logger
    )
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITenantStore tenantStore)
    {
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await _next(context);
            return;
        }

        await ResolveTenantId(context, tenantStore)
            .FoldAsync(
                onSuccess: async (tenantId) =>
                {
                    context.Items["tenant_id"] = tenantId;
                    await _next(context);
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
    private async Task<AppResult<Guid>> ResolveTenantId(HttpContext context, ITenantStore tenantStore)
    {
        var headerValue = context.Request.Headers[TenantHeaderName].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(headerValue))
        {
            _logger.LogWarning("Header {Header} ausente na requisição", TenantHeaderName);
            return AppFailure.InvalidRequest("Empresa não definida na requisição. Contate o suporte.");
        }

        if (Guid.TryParse(headerValue, out var tenantId))
        {
            return await tenantStore.TenantExistsAsync(tenantId)
                ? tenantId
                : AppFailure.InvalidRequest("Empresa não encontrada. Contate o suporte.");
        }

        var id = await tenantStore.GetTenantIdBySlugAsync(headerValue);
        if (id.HasValue)
            return id;

        return AppFailure.InvalidRequest("Empresa não encontrada. contate o suporte.");
    }
}
