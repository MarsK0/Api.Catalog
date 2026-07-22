using Api.Catalog.Api.Constants;
using Api.Catalog.Api.Models;
using Api.Catalog.Domain;
using Api.Catalog.Infrastructure.Contracts;
using System.Net;
using System.Text.Json;

namespace Api.Catalog.Api.Middlewares;

public class TenantResolverMiddleware(
    RequestDelegate next
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
        var cancellationToken = context.RequestAborted;

        var tenantHeader = context.Request.Headers[ConstantValues.TenantHeaderName].FirstOrDefault();
        var isPlatformContext =
            bool.TryParse(context.Request.Headers[ConstantValues.PlatformHeaderName].FirstOrDefault(), out var value) && value;

        tenantHeader = string.IsNullOrWhiteSpace(tenantHeader) ? null : tenantHeader;

        //Nenhum header de contexto informado
        if (!isPlatformContext && string.IsNullOrWhiteSpace(tenantHeader))
            return AppFailure.InvalidRequest("Empresa não definida na requisição. Contate o suporte.");

        //Tenant informado como Guid
        if (Guid.TryParse(tenantHeader, out Guid tenantId))
        {
            if (!(await tenantStore.TenantExistsAsync(tenantId, cancellationToken)))//Tenant não existe
                return AppFailure.InvalidRequest("Empresa não encontrada. Contate o suporte.");

            if (isPlatformContext)//Tenant informado no contexto da plataforma
                return new HttpTenantContextData(tenantId, true);

            //Tenant informado em contexto normal
            return new HttpTenantContextData(tenantId, false);
        }

        //Tenant não informado
        if (tenantHeader is null)
        {
            //Em contexto normal
            if (!isPlatformContext)
                return AppFailure.InvalidRequest("Empresa não encontrada. Contate o suporte.");
            //Em contexto plataforma
            return new HttpTenantContextData(null, true);
        }

        //Tenant informado como slug
        var id = await tenantStore.GetTenantIdBySlugAsync(tenantHeader, cancellationToken);
        if (id.HasValue)
        {
            //Em contexto de plataforma
            if (isPlatformContext)
                return new HttpTenantContextData(id, true);
            //Em contexto normal
            return new HttpTenantContextData(id, false);
        }

        return AppFailure.InvalidRequest("Empresa não encontrada. contate o suporte.");
    }
}
