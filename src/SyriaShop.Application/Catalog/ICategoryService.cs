using SyriaShop.Application.Catalog;

namespace SyriaShop.Application.Catalog;

public interface ICategoryService
{
    Task<PagedResponse<CategoryDetailResponse>> ListAsync(int page, int pageSize, bool includeInactive = false, CancellationToken cancellationToken = default);

    Task<CategoryDetailResponse> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<CategoryDetailResponse> CreateAsync(CategoryCreateRequest request, CancellationToken cancellationToken = default);

    Task<CategoryDetailResponse> UpdateAsync(Guid id, CategoryUpdateRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
