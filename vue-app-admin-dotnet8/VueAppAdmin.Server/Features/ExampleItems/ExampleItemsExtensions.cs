namespace VueAppAdmin.Server.Features.ExampleItems;

public static class ExampleItemsExtensions
{
    public static IServiceCollection AddExampleItemsFeature(this IServiceCollection services)
    {
        services.AddScoped<IExampleItemsService, ExampleItemsService>();

        return services;
    }
}
