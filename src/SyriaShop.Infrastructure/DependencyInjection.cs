using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SyriaShop.Application;
using SyriaShop.Application.Catalog;
using SyriaShop.Application.Identity;
using SyriaShop.Infrastructure.Identity;

namespace SyriaShop.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException(
                "Connection string 'Default' was not found. Configure ConnectionStrings__Default in your local .env file (see .env.example).");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<ICategoryRepository, Repositories.CategoryRepository>();
        services.AddScoped<IProductRepository, Repositories.ProductRepository>();
        services.AddScoped<IRefreshTokenRepository, Repositories.RefreshTokenRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddIdentityCore<ApplicationUser>(options => options.User.RequireUniqueEmail = true)
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager();

        services.AddTransient<AccessTokenGenerator>();
        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }

    public static async Task SeedIdentityAsync(this IServiceProvider services, IConfiguration configuration)
    {
        await using var scope = services.CreateAsyncScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        await IdentitySeeder.SeedAsync(userManager, roleManager, configuration, logger);
    }
}
