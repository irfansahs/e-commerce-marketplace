using System.Text;
using Marketplace.BuildingBlocks.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

const string ServiceName = "gateway";

var builder = WebApplication.CreateBuilder(args);
builder.AddMarketplaceLogging(ServiceName);

var jwtEnabled = builder.Configuration.GetValue("Jwt:Enabled", false);
var ocelotFile = jwtEnabled ? "ocelot.secure.json" : "ocelot.json";
builder.Configuration.AddJsonFile(ocelotFile, optional: false, reloadOnChange: true);

ConfigureJwtAuthentication(builder, jwtEnabled);

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.UseMarketplaceCorrelationId();
app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    if (context.Request.Path.Equals("/health", StringComparison.OrdinalIgnoreCase))
    {
        await Results.Ok(new { status = "Healthy", service = ServiceName }).ExecuteAsync(context);
        return;
    }

    await next();
});

try
{
    Log.Information("Starting {Service}", ServiceName);
    await app.UseOcelot();
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

static void ConfigureJwtAuthentication(WebApplicationBuilder builder, bool jwtEnabled)
{
    if (!jwtEnabled)
    {
        return;
    }

    var jwtSection = builder.Configuration.GetSection("Jwt");
    var signingKey = jwtSection["SigningKey"];
    if (string.IsNullOrWhiteSpace(signingKey))
    {
        throw new InvalidOperationException("Jwt:SigningKey is required when Jwt:Enabled is true.");
    }

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer("Bearer", options =>
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
