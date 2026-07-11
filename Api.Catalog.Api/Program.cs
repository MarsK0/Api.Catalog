using Api.Catalog.Api.Authorization;
using Api.Catalog.Api.Configurations;
using Api.Catalog.Api.Contexts;
using Api.Catalog.Api.Middlewares;
using Api.Catalog.Application;
using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Contracts.Contexts;
using Api.Catalog.Domain.Entities;
using Api.Catalog.Domain.Models;
using Api.Catalog.Infrastructure;
using Api.Catalog.Infrastructure.Persistence.PostgreSQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.Text;

try
{
    var builder = WebApplication.CreateBuilder(args);

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();
    builder.Host.UseSerilog();

    builder.Services.AddControllers();

    builder.Services.AddOpenApi();

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ITenantContext, HttpTenantContext>();
    builder.Services
        .AddInfrastructure(builder.Configuration)
        .AddApplication();

    builder.Services.AddCorsPolicies(builder.Configuration, builder.Environment);
    builder.Services.AddRateLimiterPolicies();

    var jwtKey = builder.Configuration["Jwt:Key"]
        ?? throw new InvalidOperationException("Chave JWT não configurada");
    if (jwtKey.Length < 32)
        throw new InvalidOperationException("A chave JWT deve ter ao menos 32 caracteres");

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = true;
            options.SaveToken = false;
            options.MapInboundClaims = false;
            options.IncludeErrorDetails = false;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
                RequireSignedTokens = true
            };

            options.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    context.Response.Headers["WWW-Authenticate"] = "Bearer";
                    return Task.CompletedTask;
                }
            };
        });

    builder.Services.AddAuthorization(options =>
    {
        foreach (var permission in AppPermissions.GetAll)
        {
            var policy = PermissionPolicies.Name(permission);
            options.AddPolicy(
                policy,
                p => p.RequireAuthenticatedUser().AddRequirements(new PermissionRequirement(permission))
            );
        }

        foreach (var module in AppModules.All)
        {
            var policy = TenantModulesPolicies.Name(module);
            options.AddPolicy(
                policy,
                m => m.RequireAuthenticatedUser().AddRequirements(new TenantModuleRequirement(module))
            );
        }
    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        if (!await db.PlatformMembership.AnyAsync())
        {
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var ownerName = config["Owner:Name"] ?? throw new InvalidOperationException("MasterUser Name não definido.");
            var ownerEmail = config["Owner:Email"] ?? throw new InvalidOperationException("MasterUser Email não definido.");
            var ownerPassword = config["Owner:Password"] ?? throw new InvalidOperationException("MasterUser Password não definido.");

            var personResult = Person.Create(ownerName, ownerEmail, null);
            if (!personResult.IsSuccess)
                throw new InvalidOperationException($"Falha ao criar Owner: {personResult.Failure.Message}");

            var person = personResult.Value;
            db.Persons.Add(person);

            var passwordHashService = scope.ServiceProvider.GetRequiredService<IPasswordHashService>();

            var accountResult = Account.Create(person.Id, passwordHashService.GenerateHash(ownerPassword));
            if (!accountResult.IsSuccess)
                throw new InvalidOperationException($"Falha ao criar Owner Account: {accountResult.Failure.Message}");

            var account = accountResult.Value;
            db.Accounts.Add(account);

            var platformMembership =

            await db.SaveChangesAsync(CancellationToken.None);
        }
    }

    app.UseHttpsRedirection();

    app.UseMiddleware<ErrorHandlerMiddleware>();

    app.UseRouting();
    app.UseCors(CorsPolicies.DefaultPolicy);

    app.UseRateLimiter();

    app.UseAuthentication();
    app.UseMiddleware<TenantResolverMiddleware>();
    app.UseAuthorization();

    app.MapControllers().RequireRateLimiting(RateLimitPolicies.General);

    await app.RunAsync();

}
catch (Exception ex)
{
    Log.Fatal(ex, "A aplicação encerrou inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}