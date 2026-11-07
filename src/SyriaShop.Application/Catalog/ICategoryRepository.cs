using SyriaShop.Application.Catalog;
using SyriaShop.Domain;

namespace SyriaShop.Application.Catalog;

public interface ICategoryRepository
{
    Task<PagedResponse<Category>> ListAsync(int page, int pageSize, bool includeInactive, CancellationToken cancellationToken = default);

    Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> SlugExistsAsync(string slug, Guid? excludeId, CancellationToken cancellationToken = default);

    Task<ICollection<Category>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default);

    Task<int> CountProductsAsync(Guid categoryId, CancellationToken cancellationToken = default);

    void Add(Category category);

    void Remove(Category category);
}
