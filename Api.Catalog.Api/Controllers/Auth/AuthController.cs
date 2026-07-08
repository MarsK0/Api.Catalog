using Api.Catalog.Api.Configurations;
using Api.Catalog.Application.Models;
using Api.Catalog.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Catalog.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator) : CatalogBaseController
{
    private const string RefreshCookieName = "_prx_ctlg_rt";

    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitPolicies.Login)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        return await mediator.Send(command, ct)
            .FoldAsync(onSuccess: HandleLoginResponse, onFailure: HandleFailure);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitPolicies.Login)]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        if (!Request.Cookies.TryGetValue(RefreshCookieName, out var tokenValue) || string.IsNullOrWhiteSpace(tokenValue))
            return Unauthorized(new { Message = "Nenhuma sessão ativa encontrada." });

        return await mediator.Send(new RefreshTokenCommand(tokenValue))
            .FoldAsync(onSuccess: HandleLoginResponse, onFailure: HandleFailure);
    }
    [HttpPost("logout")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitPolicies.Login)]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        Request.Cookies.TryGetValue(RefreshCookieName, out var tokenValue);
        Response.Cookies.Delete(RefreshCookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Path = "/api/auth"
        });
        return NoContent();
    }
    private IActionResult HandleLoginResponse(LoginResponse loginResponse)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Path = "/api/auth"
        };
        if (loginResponse.RememberMe)
            cookieOptions.Expires = loginResponse.RefreshTokenExpires;

        Response.Cookies.Append(RefreshCookieName, loginResponse.RefreshTokenValue, cookieOptions);
        return Ok(loginResponse.Result);
    }
}

