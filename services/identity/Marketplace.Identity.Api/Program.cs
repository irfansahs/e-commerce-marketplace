using System.Text;
using Marketplace.Identity.Api.Auth;
using Marketplace.BuildingBlocks.Health;
using Marketplace.BuildingBlocks.Localization;
using Marketplace.BuildingBlocks.Logging;
using Marketplace.Identity.Api.Data;
using Marketplace.Identity.Api.Endpoints;
using Marketplace.Identity.Api.Messaging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

const string ServiceName = "identity";

var builder = WebApplication.CreateBuilder(args);
builder.AddMarketplaceLogging(ServiceName);
builder.Services.AddMarketplaceLocalization();

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(IdentityConnectionString.Resolve(builder.Configuration)));

builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddSingleton<IIntegrationEventPublisher, RabbitMqIntegrationEventPublisher>();

ConfigureJwtAuthentication(builder);

var app = builder.Build();

app.UseMarketplaceLocalization();
app.UseMarketplaceCorrelationId();
app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapMarketplaceHealth(ServiceName);
app.MapAuthEndpoints();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    var applyMigrations = app.Environment.IsDevelopment()
        || app.Configuration.GetValue("Database:ApplyMigrationsOnStartup", false);
    if (applyMigrations)
    {
        await db.Database.MigrateAsync();
        await IdentityDbSeeder.SeedAsync(db);
    }
}

try
{
    Log.Information("Starting {Service}", ServiceName);
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "{Service} terminated unexpectedly", ServiceName);
    throw;
}
finally
{
    Log.CloseAndFlush();
}

static void ConfigureJwtAuthentication(WebApplicationBuilder builder)
{
    var jwtSection = builder.Configuration.GetSection("Jwt");
    var signingKey = jwtSection["SigningKey"]
        ?? throw new InvalidOperationException("Jwt:SigningKey is required.");

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSection["Issuer"] ?? "marketplace-identity",
                ValidAudience = jwtSection["Audience"] ?? "marketplace-api",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
            };
        });

    builder.Services.AddAuthorization();
}
