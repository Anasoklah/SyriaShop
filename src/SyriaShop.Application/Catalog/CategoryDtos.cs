namespace SyriaShop.Application.Catalog;

public record CategoryDetailResponse(Guid Id, string Slug, string NameEn, string NameAr, bool IsActive, DateTimeOffset CreatedAt);

public sealed record CategoryCreateRequest(string NameEn, string NameAr, string? Slug, bool IsActive = true);

public sealed record CategoryUpdateRequest(string NameEn, string NameAr, string? Slug, bool IsActive);
