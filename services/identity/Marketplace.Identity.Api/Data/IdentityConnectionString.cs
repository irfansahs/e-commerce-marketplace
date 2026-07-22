using Microsoft.Data.SqlClient;

namespace Marketplace.Identity.Api.Data;

public static class IdentityConnectionString
{
    public static string Resolve(IConfiguration configuration)
    {
        var configured = configuration.GetConnectionString("Identity")
            ?? throw new InvalidOperationException("ConnectionStrings:Identity is not configured.");

        var builder = new SqlConnectionStringBuilder(configured);
        var envPassword = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD");
        if (!string.IsNullOrWhiteSpace(envPassword))
        {
            builder.Password = envPassword;
        }

        var hostPort = Environment.GetEnvironmentVariable("MSSQL_HOST_PORT");
        if (!string.IsNullOrWhiteSpace(hostPort))
        {
            builder.DataSource = $"localhost,{hostPort.Trim()}";
        }

        return builder.ConnectionString;
    }
}
