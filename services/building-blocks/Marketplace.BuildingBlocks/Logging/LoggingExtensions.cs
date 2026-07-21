using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.Graylog.Core.Transport;

namespace Marketplace.BuildingBlocks.Logging;

public static class LoggingExtensions
{
    /// <summary>
    /// Configures Serilog with console + optional GELF UDP sink. App starts even if Graylog is down (UDP).
    /// </summary>
    public static WebApplicationBuilder AddMarketplaceLogging(
        this WebApplicationBuilder builder,
        string serviceName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);

        var configuration = builder.Configuration;
        var gelfHost = configuration["Gelf:Host"] ?? "localhost";
        var gelfPort = configuration.GetValue("Gelf:Port", 12201);
        var gelfEnabled = configuration.GetValue("Gelf:Enabled", true);

        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("service", serviceName)
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .WriteTo.Console();

        if (gelfEnabled)
        {
            loggerConfig = loggerConfig.WriteTo.Graylog(new GraylogSinkOptions
            {
                HostnameOrAddress = gelfHost,
                Port = gelfPort,
                TransportType = TransportType.Udp,
                Facility = serviceName
            });
        }

        Log.Logger = loggerConfig.CreateLogger();
        builder.Host.UseSerilog();
        builder.Services.AddSingleton(new MarketplaceServiceInfo(serviceName));

        return builder;
    }
}

public sealed record MarketplaceServiceInfo(string ServiceName);
