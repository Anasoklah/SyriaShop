using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using SyriaShop.Application.Catalog;

namespace SyriaShop.IntegrationTests;

public sealed class ProductEndpointsTests(CatalogApiFactory factory) : CatalogTestBase(factory)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task GetAll_ReturnsSeededProductsAndHonorsPaging()
    {
        var first = await HttpClient.GetFromJsonAsync<PagedResponse<ProductListItemResponse>>(
            "/api/products?page=1&pageSize=5", JsonOptions);
        var second = await HttpClient.GetFromJsonAsync<PagedResponse<ProductListItemResponse>>(
            "/api/products?page=2&pageSize=5", JsonOptions);

        Assert.NotNull(first);
        Assert.NotNull(second);
        Assert.True(first!.TotalCount >= 16);
        Assert.Equal(5, first.Items.Count);
        Assert.Equal(5, second!.Items.Count);
        Assert.Equal((int)Math.Ceiling(first.TotalCount / 5d), first.TotalPages);
        Assert.NotEqual(first.Items[0].Id, second.Items[0].Id);
    }

    [Fact]
    public async Task GetAll_WithPriceRange_FiltersResults()
    {
        var products = await HttpClient.GetFromJsonAsync<PagedResponse<ProductListItemResponse>>(
            "/api/products?pageSize=100&minPrice=50000&maxPrice=100000", JsonOptions);

        Assert.NotNull(products);
        Assert.All(products!.Items, p => Assert.InRange(p.Price, 50000m, 100000m));
        Assert.Contains(products.Items, p => p.Slug == "vitamin-c-serum");
    }

    [Fact]
    public async Task GetAll_WithSearch_MatchesArabicName()
    {
        var body = await HttpClient.GetStringAsync("/api/products?pageSize=100&search=فستق");

        Assert.Contains("roasted-pistachio-500g", body);
    }

    [Fact]
    public async Task GetAll_SortByPriceDescending_PutsWatchFirst()
    {
        var products = await HttpClient.GetFromJsonAsync<PagedResponse<ProductListItemResponse>>(
            "/api/products?pageSize=100&sortBy=PriceDescending", JsonOptions);

        Assert.NotNull(products);
        Assert.Equal("smart-watch-fit", products!.Items[0].Slug);
    }

    [Fact]
    public async Task GetAll_WithCategorySlug_ReturnsOnlyThatCategory()
    {
        var products = await HttpClient.GetFromJsonAsync<PagedResponse<ProductListItemResponse>>(
            "/api/products?pageSize=100&categorySlug=stationery", JsonOptions);

        Assert.NotNull(products);
        Assert.Single(products!.Items);
        Assert.Equal("hardcover-notebook-a5", products.Items[0].Slug);
    }

    [Fact]
    public async Task CreateReadUpdateDelete_RoundTrip_WithAdminToken()
    {
        var adminClient = await CreateAdminClientAsync();
        var category = await CreateCategoryAsync(adminClient);

        var createResponse = await adminClient.PostAsJsonAsync("/api/products", new ProductCreateRequest(
            category.Id, "Roundtrip Widget", "منتج دورة", Price: 75000m, Stock: 12, DescriptionEn: "A test widget"), JsonOptions);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = (await createResponse.Content.ReadFromJsonAsync<ProductDetailResponse>(JsonOptions))!;

        var getResponse = await adminClient.GetAsync($"/api/products/{created.Slug}");
        getResponse.EnsureSuccessStatusCode();

        var update = await adminClient.PutAsJsonAsync($"/api/products/{created.Id}", new ProductUpdateRequest(
            category.Id, "Roundtrip Widget V2", "منتج دورة 2", Price: 90000m, Stock: 20, IsActive: true), JsonOptions);
        update.EnsureSuccessStatusCode();

        var updated = (await update.Content.ReadFromJsonAsync<ProductDetailResponse>(JsonOptions))!;
        Assert.Equal(90000m, updated.Price);
        Assert.Equal(20, updated.Stock);

        var delete = await adminClient.DeleteAsync($"/api/products/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);

        var gone = await adminClient.GetAsync($"/api/products/{created.Slug}");
        Assert.Equal(HttpStatusCode.NotFound, gone.StatusCode);
    }

    [Fact]
    public async Task Create_WithUnknownCategoryId_Returns404()
    {
        var adminClient = await CreateAdminClientAsync();

        var response = await adminClient.PostAsJsonAsync("/api/products", new ProductCreateRequest(
            Guid.NewGuid(), "Ghost Product", "منتج وهمي", Price: 1000m, Stock: 1), JsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithNegativeStock_Returns400ProblemDetails()
    {
        var adminClient = await CreateAdminClientAsync();
        var category = await CreateCategoryAsync(adminClient);

        var response = await adminClient.PostAsJsonAsync("/api/products", new ProductCreateRequest(
            category.Id, "Broken Product", "منتج خاطئ", Price: 1000m, Stock: -3), JsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType!.MediaType);
    }

    [Fact]
    public async Task GetBySlug_UnknownSlug_Returns404()
    {
        var response = await HttpClient.GetAsync("/api/products/does-not-exist-xyz");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<CategoryDetailResponse> CreateCategoryAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/categories", new CategoryCreateRequest(
            $"Product Test Cat {Guid.NewGuid():N}", "فئة منتجات", $"product-test-{Guid.NewGuid():N}"), JsonOptions);

        return (await response.Content.ReadFromJsonAsync<CategoryDetailResponse>(JsonOptions))!;
    }
}
