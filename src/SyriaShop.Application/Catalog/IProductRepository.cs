using SyriaShop.Domain;

namespace SyriaShop.Application.Catalog;

public interface IProductRepository
{
    Task<PagedResponse<Product>> ListAsync(ProductListQuery query, CancellationToken cancellationToken = default);

    Task<Product?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> SlugExistsAsync(string slug, Guid? excludeId, CancellationToken cancellationToken = default);

    void Add(Product product);

    void Remove(Product product);
}
