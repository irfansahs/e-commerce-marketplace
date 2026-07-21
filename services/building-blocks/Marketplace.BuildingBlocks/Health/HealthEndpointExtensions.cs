using Marketplace.BuildingBlocks.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.BuildingBlocks.Health;

public static class HealthEndpointExtensions
{
    /// <summary>
    /// Maps GET /health → 200 JSON { status, service }.
    /// </summary>
    public static IEndpointRouteBuilder MapMarketplaceHealth(
        this IEndpointRouteBuilder endpoints,
        string? serviceName = null)
    {
        var name = serviceName
            ?? endpoints.ServiceProvider.GetService<MarketplaceServiceInfo>()?.ServiceName
            ?? "unknown";

        endpoints.MapGet("/health", () => Results.Ok(new
        {
            status = "Healthy",
            service = name
        }));

        return endpoints;
    }
}
