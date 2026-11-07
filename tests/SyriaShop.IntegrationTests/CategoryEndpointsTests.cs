using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using SyriaShop.Application.Catalog;
using System.Text.Json;

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
    public async Task CreateUpdateDelete_RoundTrip()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];

        var createdResponse = await HttpClient.PostAsJsonAsync("/api/categories", new CategoryCreateRequest(
            "Roundtrip Category", "فئة دورة كاملة", $"roundtrip-{suffix}"), JsonOptions);

        Assert.Equal(HttpStatusCode.Created, createdResponse.StatusCode);

        var created = await createdResponse.Content.ReadFromJsonAsync<CategoryDetailResponse>(JsonOptions)
            ?? throw new InvalidOperationException("Category creation did not return a body.");

        var getResponse = await HttpClient.GetAsync($"/api/categories/{created.Slug}");
        getResponse.EnsureSuccessStatusCode();

        var updateResponse = await HttpClient.PutAsJsonAsync($"/api/categories/{created.Id}",
            new CategoryUpdateRequest("Renamed Roundtrip", "فئة معدلة", created.Slug, IsActive: false), JsonOptions);
        updateResponse.EnsureSuccessStatusCode();

        var updated = await updateResponse.Content.ReadFromJsonAsync<CategoryDetailResponse>(JsonOptions);
        Assert.Equal("Renamed Roundtrip", updated!.NameEn);
        Assert.False(updated.IsActive);

        var deleteResponse = await HttpClient.DeleteAsync($"/api/categories/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var gone = await HttpClient.GetAsync($"/api/categories/{created.Slug}");
        Assert.Equal(HttpStatusCode.NotFound, gone.StatusCode);
    }

    [Fact]
    public async Task Create_WithDuplicateSlug_Returns409ProblemDetails()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];

        var first = await CreateSharedCategory(suffix);

        var duplicate = await HttpClient.PostAsJsonAsync("/api/categories", new CategoryCreateRequest(
            "Duplicate", "مكرر", first.Slug), JsonOptions);

        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
        Assert.Equal("application/problem+json", duplicate.Content.Headers.ContentType!.MediaType);
        Assert.Contains(first.Slug, await duplicate.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Delete_CategoryWithProducts_Returns409()
    {
        var electronics = await GetElectronicsCategoryAsync();

        var deleteResponse = await HttpClient.DeleteAsync($"/api/categories/{electronics.Id}");

        Assert.Equal(HttpStatusCode.Conflict, deleteResponse.StatusCode);

        var after = await HttpClient.GetAsync($"/api/categories/{electronics.Slug}");
        Assert.Equal(HttpStatusCode.OK, after.StatusCode);
    }

    [Fact]
    public async Task Create_WithEmptyName_Returns400ProblemDetails()
    {
        var response = await HttpClient.PostAsJsonAsync("/api/categories", new CategoryCreateRequest(
            "   ", "فارغ", $"empty-{Guid.NewGuid():N}"), JsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType!.MediaType);
    }

    private async Task<CategoryDetailResponse> CreateSharedCategory(string suffix)
    {
        var request = new CategoryCreateRequest(
            $"Shared Category {suffix}",
            $"فئة اختبار {suffix}",
            $"shared-{suffix}");

        var response = await HttpClient.PostAsJsonAsync("/api/categories", request, JsonOptions);

        return (await response.Content.ReadFromJsonAsync<CategoryDetailResponse>(JsonOptions))!;
    }

    private async Task<CategoryDetailResponse> GetElectronicsCategoryAsync()
    {
        var response = await HttpClient.GetAsync("/api/categories?pageSize=100");

        var body = await response.Content.ReadFromJsonAsync<PagedResponse<CategoryDetailResponse>>(JsonOptions);

        return body!.Items.First(c => c.Slug == "electronics");
    }
}
