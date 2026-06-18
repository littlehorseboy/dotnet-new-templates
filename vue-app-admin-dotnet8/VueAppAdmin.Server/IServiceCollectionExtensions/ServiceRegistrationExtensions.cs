using VueAppAdmin.Server.Services;
using VueAppAdmin.Server.Services.ExampleItems;

namespace VueAppAdmin.Server.IServiceCollectionExtensions;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<JwtService>();
        services.AddScoped<ExampleItemsService>();
        return services;
    }
}
