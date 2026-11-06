using System.Net.Http.Json;
using System.Text.Json;
using SyriaShop.Application.Catalog;

namespace SyriaShop.IntegrationTests;

public abstract class CatalogTestBase : IClassFixture<CatalogApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    protected CatalogTestBase(CatalogApiFactory factory)
    {
        HttpClient = factory.CreateClient();
    }

    protected HttpClient HttpClient { get; }

    protected static async Task<CategoryDetailResponse> CreateTestCategoryAsync(HttpClient client)
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var request = new CategoryCreateRequest(
            NameEn: $"Shared Category {suffix}",
            NameAr: $"فئة مشتركة {suffix}",
            Slug: $"shared-category-{suffix}",
            IsActive: true);

        var response = await client.PostAsJsonAsync("/api/categories", request, JsonOptions);

        return await response.Content.ReadFromJsonAsync<CategoryDetailResponse>(JsonOptions)
            ?? throw new InvalidOperationException("Category creation did not return a body.");
    }
}
