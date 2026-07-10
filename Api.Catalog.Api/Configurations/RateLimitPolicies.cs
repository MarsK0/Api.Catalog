using System.Text.Json;
using System.Threading.RateLimiting;

namespace Api.Catalog.Api.Configurations;

internal static class RateLimitPolicies
{
    public const string Login = "login";
    public const string General = "general";

    public static IServiceCollection AddRateLimiterPolicies(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = async (context, ct) =>
            {
                context.HttpContext.Response.Headers["Retry-After"] = "60";
                context.HttpContext.Response.ContentType = "application/json";
                await context.HttpContext.Response.WriteAsync(
                    JsonSerializer.Serialize(new { Message = "Muitas tentativas. Tente novamente em alguns instantes." }),
                    ct
                );
            };

            options.AddPolicy(Login, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: ClientKey(context, "login"),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 1,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        AutoReplenishment = true,

                    }
                )
            );

            options.AddPolicy(General, context =>
                RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey: ClientIp(context),
                    factory: _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 200,
                        Window = TimeSpan.FromMinutes(1),
                        SegmentsPerWindow = 6,
                        QueueLimit = 0,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        AutoReplenishment = true
                    }
                )
            );

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey: ClientIp(context),
                    factory: _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 600,
                        Window = TimeSpan.FromMinutes(1),
                        SegmentsPerWindow = 6,
                        QueueLimit = 0,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        AutoReplenishment = true
                    }
                )
            );
        });

        return services;
    }

    private static string ClientKey(HttpContext context, string suffix)
    {
        var ip = ClientIp(context);
        var userId = context.User?.Identity?.IsAuthenticated == true
            ? context.User?.FindFirst("sub")?.Value ?? "anonymous"
            : "anonymous";

        return $"{suffix}:{ip}:{userId}";
    }
    private static string ClientIp(HttpContext context) =>
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}
