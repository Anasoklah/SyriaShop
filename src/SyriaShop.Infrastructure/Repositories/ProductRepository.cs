using Microsoft.EntityFrameworkCore;
using SyriaShop.Application.Catalog;
using SyriaShop.Domain;

namespace SyriaShop.Infrastructure.Repositories;

public sealed class ProductRepository(AppDbContext context) : IProductRepository
{
    public async Task<PagedResponse<Product>> ListAsync(ProductListQuery query, CancellationToken cancellationToken = default)
    {
        var baseQuery = context.Products.AsNoTracking();

        if (!query.IncludeInactive)
        {
            baseQuery = baseQuery.Where(p => p.IsActive);
        }

        baseQuery = FilterCategory(baseQuery, query);
        baseQuery = FilterPriceRange(baseQuery, query);
        baseQuery = FilterSearch(baseQuery, query);

        var ordered = query.SortBy switch
        {
            ProductSortBy.PriceAscending => baseQuery.OrderBy(p => p.Price),
            ProductSortBy.PriceDescending => baseQuery.OrderByDescending(p => p.Price),
            ProductSortBy.Name => baseQuery.OrderBy(p => p.NameEn),
            ProductSortBy.Newest => baseQuery.OrderByDescending(p => p.CreatedAt),
            _ => baseQuery.OrderByDescending(p => p.CreatedAt)
        };

        var totalCount = await ordered.CountAsync(cancellationToken);

        var items = await ordered
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<Product>(items, query.Page, query.PageSize, totalCount);
    }

    public Task<Product?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default) =>
        context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<bool> SlugExistsAsync(string slug, Guid? excludeId, CancellationToken cancellationToken = default) =>
        context.Products.AnyAsync(
            p => p.Slug == slug && (excludeId == null || p.Id != excludeId),
            cancellationToken);

    public void Add(Product product) => context.Add(product);

    public void Remove(Product product) => context.Remove(product);

    private IQueryable<Product> FilterCategory(IQueryable<Product> query, ProductListQuery listQuery)
    {
        if (listQuery.CategoryId is { } categoryId)
        {
            return query.Where(p => p.CategoryId == categoryId);
        }

        if (listQuery.CategorySlug is { Length: > 0 } categorySlug)
        {
            return query.Where(p => context.Categories.Any(c => c.Id == p.CategoryId && c.Slug == categorySlug));
        }

        return query;
    }

    private static IQueryable<Product> FilterPriceRange(IQueryable<Product> query, ProductListQuery listQuery)
    {
        if (listQuery.MinPrice is { } minPrice)
        {
            query = query.Where(p => p.Price >= minPrice);
        }

        if (listQuery.MaxPrice is { } maxPrice)
        {
            query = query.Where(p => p.Price <= maxPrice);
        }

        return query;
    }

    private static IQueryable<Product> FilterSearch(IQueryable<Product> query, ProductListQuery listQuery)
    {
        var search = listQuery.Search?.Trim();

        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var pattern = $"%{search}%";

        return query.Where(p =>
            EF.Functions.Like(p.NameEn, pattern)
            || EF.Functions.Like(p.NameAr, pattern)
            || EF.Functions.Like(p.DescriptionEn!, pattern)
            || EF.Functions.Like(p.DescriptionAr!, pattern));
    }
}
