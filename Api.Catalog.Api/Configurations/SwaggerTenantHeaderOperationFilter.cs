using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Catalog.Api.Configurations;

public class SwaggerTenantHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var path = context.ApiDescription.RelativePath ?? string.Empty;

        // Ignora paths que não comecem com "api", pois o resolver valida o cabeçalho apenas em /api
        if (!path.StartsWith("api", StringComparison.OrdinalIgnoreCase))
            return;

        if (operation.Parameters is null)
            operation.Parameters = new List<IOpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            // O mesmo nome de TenantHeaderName no TenantResolverMiddleware
            Name = "Catalog-Tenant",
            In = ParameterLocation.Header,
            Required = true,
            Schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String
            },
            Description = "ID ou Slug do tenant"
        });
    }
}
