using SyriaShop.Application.Catalog;

namespace SyriaShop.Application.Catalog;

public interface IProductService
{
    Task<PagedResponse<ProductListItemResponse>> ListAsync(ProductListQuery query, CancellationToken cancellationToken = default);

    Task<ProductDetailResponse> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<ProductDetailResponse> CreateAsync(ProductCreateRequest request, CancellationToken cancellationToken = default);

    Task<ProductDetailResponse> UpdateAsync(Guid id, ProductUpdateRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
