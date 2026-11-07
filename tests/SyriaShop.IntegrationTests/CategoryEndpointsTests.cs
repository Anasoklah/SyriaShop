using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using SyriaShop.Application.Catalog;

namespace SyriaShop.IntegrationTests;

public sealed class CategoryEndpointsTests(CatalogApiFactory factory) : CatalogTestBase(factory)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task GetAll_ReturnsSeededCategories()
    {
        var response = await HttpClient.GetAsync("/api/categories?pageSize=100");

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<PagedResponse<CategoryDetailResponse>>(JsonOptions);

        Assert.NotNull(body);
        Assert.True(body!.TotalCount >= 6);
        Assert.Contains(body.Items, c => c.Slug == "electronics");
        Assert.Contains(body.Items, c => c.NameAr == "الطعام والمونة");
    }

    [Fact]
    public async Task CreateUpdateDelete_RoundTrip_WithAdminToken()
    {
        var adminClient = await CreateAdminClientAsync();
        var suffix = Guid.NewGuid().ToString("N")[..8];

        var createdResponse = await adminClient.PostAsJsonAsync("/api/categories", new CategoryCreateRequest(
            "Roundtrip Category", "فئة دورة كاملة", $"roundtrip-{suffix}"), JsonOptions);

        Assert.Equal(HttpStatusCode.Created, createdResponse.StatusCode);

        var created = await createdResponse.Content.ReadFromJsonAsync<CategoryDetailResponse>(JsonOptions)
            ?? throw new InvalidOperationException("Category creation did not return a body.");

        var getResponse = await adminClient.GetAsync($"/api/categories/{created.Slug}");
        getResponse.EnsureSuccessStatusCode();

        var updateResponse = await adminClient.PutAsJsonAsync($"/api/categories/{created.Id}",
            new CategoryUpdateRequest("Renamed Roundtrip", "فئة معدلة", created.Slug, IsActive: false), JsonOptions);
        updateResponse.EnsureSuccessStatusCode();

        var updated = await updateResponse.Content.ReadFromJsonAsync<CategoryDetailResponse>(JsonOptions);
        Assert.Equal("Renamed Roundtrip", updated!.NameEn);
        Assert.False(updated.IsActive);

        var deleteResponse = await adminClient.DeleteAsync($"/api/categories/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var gone = await adminClient.GetAsync($"/api/categories/{created.Slug}");
        Assert.Equal(HttpStatusCode.NotFound, gone.StatusCode);
    }

    [Fact]
    public async Task Create_WithoutToken_Returns401()
    {
        var response = await HttpClient.PostAsJsonAsync("/api/categories", new CategoryCreateRequest(
            "Anon", "مجهول", $"anon-{Guid.NewGuid():N}"), JsonOptions);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithDuplicateSlug_Returns409ProblemDetails()
    {
        var adminClient = await CreateAdminClientAsync();
        var suffix = Guid.NewGuid().ToString("N")[..8];

        var first = await CreateSharedCategoryAsync(adminClient, suffix);

        var duplicate = await adminClient.PostAsJsonAsync("/api/categories", new CategoryCreateRequest(
            "Duplicate", "مكرر", first.Slug), JsonOptions);

        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
        Assert.Equal("application/problem+json", duplicate.Content.Headers.ContentType!.MediaType);
        Assert.Contains(first.Slug, await duplicate.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Delete_CategoryWithProducts_Returns409()
    {
        var adminClient = await CreateAdminClientAsync();

        var electronics = await GetElectronicsCategoryAsync(adminClient);

        var deleteResponse = await adminClient.DeleteAsync($"/api/categories/{electronics.Id}");

        Assert.Equal(HttpStatusCode.Conflict, deleteResponse.StatusCode);

        var after = await adminClient.GetAsync($"/api/categories/{electronics.Slug}");
        Assert.Equal(HttpStatusCode.OK, after.StatusCode);
    }

    [Fact]
    public async Task Create_WithEmptyName_Returns400ProblemDetails()
    {
        var adminClient = await CreateAdminClientAsync();

        var response = await adminClient.PostAsJsonAsync("/api/categories", new CategoryCreateRequest(
            "   ", "فارغ", $"empty-{Guid.NewGuid():N}"), JsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType!.MediaType);
    }

    private async Task<HttpClient> CreateAdminClientAsync()
    {
        await SeedIdentityAsync();

        var (email, password) = await GetAdminCredentialsAsync();
        var token = await LoginAsync(HttpClient, email, password);

        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        return client;
    }

    private async Task<CategoryDetailResponse> CreateSharedCategoryAsync(HttpClient client, string suffix)
    {
        var response = await client.PostAsJsonAsync("/api/categories", new CategoryCreateRequest(
            $"Shared Category {suffix}",
            $"فئة اختبار {suffix}",
            $"shared-{suffix}"), JsonOptions);

        return (await response.Content.ReadFromJsonAsync<CategoryDetailResponse>(JsonOptions))!;
    }

    private async Task<CategoryDetailResponse> GetElectronicsCategoryAsync(HttpClient client)
    {
        var response = await client.GetAsync("/api/categories?pageSize=100");

        var body = (await response.Content.ReadFromJsonAsync<PagedResponse<CategoryDetailResponse>>(JsonOptions))!;

        return body.Items.First(c => c.Slug == "electronics");
    }
}
