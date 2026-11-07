using Microsoft.EntityFrameworkCore;
using SyriaShop.Application.Catalog;
using SyriaShop.Domain;

namespace SyriaShop.Infrastructure.Repositories;

public sealed class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public async Task<PagedResponse<Category>> ListAsync(int page, int pageSize, bool includeInactive, CancellationToken cancellationToken = default)
    {
        var baseQuery = context.Categories.AsNoTracking();

        if (!includeInactive)
        {
            baseQuery = baseQuery.Where(c => c.IsActive);
        }

        var ordered = baseQuery.OrderBy(c => c.NameEn);

        var totalCount = await ordered.CountAsync(cancellationToken);

        var items = await ordered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<Category>(items, page, pageSize, totalCount);
    }

    public Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default) =>
        context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<bool> SlugExistsAsync(string slug, Guid? excludeId, CancellationToken cancellationToken = default) =>
        context.Categories.AnyAsync(
            c => c.Slug == slug && (excludeId == null || c.Id != excludeId),
            cancellationToken);

    public Task<int> CountProductsAsync(Guid categoryId, CancellationToken cancellationToken = default) =>
        context.Products.CountAsync(p => p.CategoryId == categoryId, cancellationToken);

    public async Task<ICollection<Category>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        return await context.Categories.AsNoTracking()
            .Where(c => ids.Contains(c.Id))
            .ToListAsync(cancellationToken);
    }

    public void Add(Category category) => context.Add(category);

    public void Remove(Category category) => context.Remove(category);
}
