using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SyriaShop.Domain;

namespace SyriaShop.Infrastructure.Identity;

public static class IdentitySeeder
{
    public const string AdminRole = "Admin";
    public const string CustomerRole = "Customer";

    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IConfiguration configuration,
        ILogger logger)
    {
        foreach (var role in new[] { AdminRole, CustomerRole })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                _ = await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

        var adminEmail = configuration.GetSection("Auth:SeedAdminEmail").Value;
        var adminPassword = configuration.GetSection("Auth:SeedAdminPassword").Value;

        if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
        {
            if (await userManager.FindByEmailAsync(adminEmail) is null)
            {
                var admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail };
                _ = await userManager.CreateAsync(admin, adminPassword);
                _ = await userManager.AddToRoleAsync(admin, AdminRole);

                logger.LogInformation("Seeded admin account {Email}.", adminEmail);
            }
        }

        var customerEmail = configuration.GetSection("Auth:SeedCustomerEmail").Value;
        var customerPassword = configuration.GetSection("Auth:SeedCustomerPassword").Value;

        if (!string.IsNullOrWhiteSpace(customerEmail) && !string.IsNullOrWhiteSpace(customerPassword))
        {
            if (await userManager.FindByEmailAsync(customerEmail) is null)
            {
                var customer = new ApplicationUser { UserName = customerEmail, Email = customerEmail };
                _ = await userManager.CreateAsync(customer, customerPassword);
                _ = await userManager.AddToRoleAsync(customer, CustomerRole);

                logger.LogInformation("Seeded customer account {Email}.", customerEmail);
            }
        }
    }
}
