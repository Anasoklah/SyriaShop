using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using SyriaShop.Application.Catalog;
using SyriaShop.Application.Identity;

namespace SyriaShop.IntegrationTests;

public sealed class AuthEndpointsTests(CatalogApiFactory factory) : CatalogTestBase(factory)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task Register_ReturnsTokenPair()
    {
        var email = $"user-{Guid.NewGuid():N}@test.local";

        var response = await HttpClient.PostAsJsonAsync("/api/auth/register", new RegisterRequest(
            email, "Pass#2026!Strong"), JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var tokens = (await response.Content.ReadFromJsonAsync<TokenPairResponse>(JsonOptions))!;

        Assert.False(string.IsNullOrWhiteSpace(tokens.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(tokens.RefreshToken));
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns400ProblemDetails()
    {
        await SeedIdentityAsync();
        var admin = await GetAdminCredentialsAsync();

        var response = await HttpClient.PostAsJsonAsync("/api/auth/register", new RegisterRequest(
            admin.Email, "Pass#2026!Another"), JsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType!.MediaType);
    }

    [Fact]
    public async Task Register_WithWeakPassword_Returns400()
    {
        var email = $"weak-{Guid.NewGuid():N}@test.local";

        var response = await HttpClient.PostAsJsonAsync("/api/auth/register", new RegisterRequest(
            email, "123"), JsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsTokenPair()
    {
        await SeedIdentityAsync();
        var admin = await GetAdminCredentialsAsync();

        var response = await HttpClient.PostAsJsonAsync("/api/auth/login", new LoginRequest(
            admin.Email, admin.Password), JsonOptions);

        response.EnsureSuccessStatusCode();

        var tokens = (await response.Content.ReadFromJsonAsync<TokenPairResponse>(JsonOptions))!;

        Assert.False(string.IsNullOrWhiteSpace(tokens.AccessToken));
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401ProblemDetails()
    {
        await SeedIdentityAsync();
        var admin = await GetAdminCredentialsAsync();

        var response = await HttpClient.PostAsJsonAsync("/api/auth/login", new LoginRequest(
            admin.Email, "Definitely!Wrong#999"), JsonOptions);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType!.MediaType);
    }

    [Fact]
    public async Task AdminEndpoints_RequireAdminRole()
    {
        await SeedIdentityAsync();
        var adminCredentials = await GetAdminCredentialsAsync();

        var customerEmail = $"plain-{Guid.NewGuid():N}@test.local";
        var registerResponse = await HttpClient.PostAsJsonAsync("/api/auth/register", new RegisterRequest(
            customerEmail, "Pass#2026!Plain"), JsonOptions);
        var customerTokens = (await registerResponse.Content.ReadFromJsonAsync<TokenPairResponse>(JsonOptions))!;

        var adminToken = await LoginAsync(HttpClient, adminCredentials.Email, adminCredentials.Password);

        var body = new CategoryCreateRequest("Role Probe", "منتج دورات الأدوار", $"role-probe-{Guid.NewGuid():N}");

        var anonymous = await HttpClient.PostAsJsonAsync("/api/categories", body, JsonOptions);
        var customer = await SendWithTokenAsync(HttpClient, customerTokens.AccessToken, body);
        var admin = await SendWithTokenAsync(HttpClient, adminToken, body);

        Assert.Equal(HttpStatusCode.Unauthorized, anonymous.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, customer.StatusCode);
        Assert.Equal(HttpStatusCode.Created, admin.StatusCode);
    }

    [Fact]
    public async Task Refresh_RotatesAndInvalidatesOldToken()
    {
        var email = $"refresh-{Guid.NewGuid():N}@test.local";
        var registerResponse = await HttpClient.PostAsJsonAsync("/api/auth/register", new RegisterRequest(
            email, "Pass#2026!Rotate"), JsonOptions);
        var original = (await registerResponse.Content.ReadFromJsonAsync<TokenPairResponse>(JsonOptions))!;

        var refreshedResponse = await HttpClient.PostAsJsonAsync("/api/auth/refresh", new RefreshRequest(
            original.RefreshToken), JsonOptions);

        refreshedResponse.EnsureSuccessStatusCode();

        var reusedOld = await HttpClient.PostAsJsonAsync("/api/auth/refresh", new RefreshRequest(
            original.RefreshToken), JsonOptions);

        Assert.Equal(HttpStatusCode.Unauthorized, reusedOld.StatusCode);
    }

    [Fact]
    public async Task Refresh_WithUnknownToken_Returns401()
    {
        var response = await HttpClient.PostAsJsonAsync("/api/auth/refresh", new RefreshRequest(
            $"unknown-{Guid.NewGuid():N}"), JsonOptions);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static async Task<HttpResponseMessage> SendWithTokenAsync(HttpClient baseClient, string token, CategoryCreateRequest body)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/categories")
        {
            Content = JsonContent.Create(body)
        };
        request.Headers.Authorization = new("Bearer", token);

        return await baseClient.SendAsync(request);
    }
}
