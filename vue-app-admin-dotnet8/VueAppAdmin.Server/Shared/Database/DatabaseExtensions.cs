using Microsoft.Data.SqlClient;
using System.Data;

namespace VueAppAdmin.Server.Shared.Database;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IDbConnection>(_ =>
            new SqlConnection(configuration.GetConnectionString("Default")));

        return services;
    }
}
