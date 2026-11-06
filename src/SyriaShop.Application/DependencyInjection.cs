using Microsoft.Extensions.DependencyInjection;

namespace SyriaShop.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<Catalog.ICategoryService, Catalog.CategoryService>();
        services.AddScoped<Catalog.IProductService, Catalog.ProductService>();

        return services;
    }
}
