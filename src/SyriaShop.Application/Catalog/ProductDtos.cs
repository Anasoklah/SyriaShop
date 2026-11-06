namespace SyriaShop.Application.Catalog;

public record ProductDetailResponse(Guid Id, Guid CategoryId, string CategorySlug, string CategoryNameEn,
    string CategoryNameAr, string Slug, string NameEn, string NameAr, string? DescriptionEn, string? DescriptionAr,
    decimal Price, int Stock, string? ImageUrl, bool IsActive, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);

public record ProductListItemResponse(Guid Id, string CategorySlug, string Slug, string NameEn, string NameAr,
    decimal Price, string? ImageUrl, bool IsActive);

public sealed record ProductCreateRequest(Guid CategoryId, string NameEn, string NameAr, decimal Price, int Stock,
    string? Slug = null, string? DescriptionEn = null, string? DescriptionAr = null, string? ImageUrl = null,
    bool IsActive = true);

public sealed record ProductUpdateRequest(Guid CategoryId, string NameEn, string NameAr, decimal Price, int Stock,
    bool IsActive, string? Slug = null, string? DescriptionEn = null, string? DescriptionAr = null,
    string? ImageUrl = null);

public enum ProductSortBy
{
    Newest = 0,
    PriceAscending = 1,
    PriceDescending = 2,
    Name = 3
}

public sealed record ProductListQuery(int Page = 1, int PageSize = 20, Guid? CategoryId = null, string? CategorySlug = null,
    decimal? MinPrice = null, decimal? MaxPrice = null, string? Search = null, ProductSortBy SortBy = ProductSortBy.Newest,
    bool IncludeInactive = false);