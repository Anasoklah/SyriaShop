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
                var roleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(role));

                if (!roleResult.Succeeded)
                {
                    logger.LogError("Failed to seed role {Role}: {Errors}", role, DescribeErrors(roleResult));
                }
            }
        }

        var adminEmail = configuration.GetSection("Auth:SeedAdminEmail").Value;
        var adminPassword = configuration.GetSection("Auth:SeedAdminPassword").Value;

        if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
        {
            if (await userManager.FindByEmailAsync(adminEmail) is null)
            {
                await SeedUserAsync(userManager, adminEmail, adminPassword, AdminRole, logger);
            }
        }

        var customerEmail = configuration.GetSection("Auth:SeedCustomerEmail").Value;
        var customerPassword = configuration.GetSection("Auth:SeedCustomerPassword").Value;

        if (!string.IsNullOrWhiteSpace(customerEmail) && !string.IsNullOrWhiteSpace(customerPassword))
        {
            if (await userManager.FindByEmailAsync(customerEmail) is null)
            {
                await SeedUserAsync(userManager, customerEmail, customerPassword, CustomerRole, logger);
            }
        }
    }

    private static async Task SeedUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string role,
        ILogger logger)
    {
        var user = new ApplicationUser { UserName = email, Email = email };

        var creation = await userManager.CreateAsync(user, password);

        if (!creation.Succeeded)
        {
            logger.LogError("Failed to seed {Role} account {Email}: {Errors}", role, email, DescribeErrors(creation));

            return;
        }

        var roleAssignment = await userManager.AddToRoleAsync(user, role);

        if (!roleAssignment.Succeeded)
        {
            logger.LogError("Failed to assign role {Role} to seeded account {Email}: {Errors}",
                role, email, DescribeErrors(roleAssignment));

            return;
        }

        logger.LogInformation("Seeded {Role} account {Email}.", role, email);
    }

    private static string DescribeErrors(IdentityResult result) =>
        string.Join(' ', result.Errors.Select(e => $"{e.Code}: {e.Description}"));
}
