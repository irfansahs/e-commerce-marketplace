using Marketplace.BuildingBlocks.Health;
using Marketplace.BuildingBlocks.Logging;
using Serilog;

const string ServiceName = "identity";

var builder = WebApplication.CreateBuilder(args);
builder.AddMarketplaceLogging(ServiceName);

var app = builder.Build();

app.UseMarketplaceCorrelationId();
app.UseSerilogRequestLogging();

app.MapMarketplaceHealth(ServiceName);

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
