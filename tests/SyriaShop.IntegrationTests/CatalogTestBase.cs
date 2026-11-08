using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using SyriaShop.Application.Identity;
using SyriaShop.Infrastructure;
using SyriaShop.Infrastructure.Identity;

namespace SyriaShop.IntegrationTests;

public abstract class CatalogTestBase : IClassFixture<CatalogApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    protected CatalogTestBase(CatalogApiFactory factory)
    {
        Factory = factory;
        HttpClient = factory.CreateClient();
    }

    protected CatalogApiFactory Factory { get; }

    protected HttpClient HttpClient { get; }

    protected async Task SeedIdentityAsync()
    {
        using var scope = Factory.Services.CreateScope();

        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        await IdentitySeeder.SeedAsync(userManager, roleManager, configuration, NullLogger.Instance);
    }

    protected async Task<(string Email, string Password)> GetAdminCredentialsAsync()
    {
        var configuration = Factory.Services.GetRequiredService<IConfiguration>();

        return (
            configuration.GetSection("Auth:SeedAdminEmail").Value!,
            configuration.GetSection("Auth:SeedAdminPassword").Value!);
    }

    protected async Task<string> LoginAsync(HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, password), JsonOptions);

        response.EnsureSuccessStatusCode();

        var tokens = await response.Content.ReadFromJsonAsync<TokenPairResponse>(JsonOptions);

        return tokens!.AccessToken;
    }

    protected async Task<HttpClient> CreateAdminClientAsync()
    {
        await SeedIdentityAsync();

        var (email, password) = await GetAdminCredentialsAsync();
        var token = await LoginAsync(HttpClient, email, password);

        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        return client;
    }

    protected async Task<HttpClient> CreateClientWithTokenAsync(string token)
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        return client;
    }
}
