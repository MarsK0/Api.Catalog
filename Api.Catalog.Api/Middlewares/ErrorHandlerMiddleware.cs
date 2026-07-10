using System.Net;
using System.Text.Json;

namespace Api.Catalog.Api.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlerMiddleware> logger
    )
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado.");
            await RespondAsync(context, HttpStatusCode.InternalServerError, new
            {
                Message = "Um erro ocorreu :/"
            });
        }
    }

    private static async Task RespondAsync(HttpContext context, HttpStatusCode statusCode, object json)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(json));
    }
}
