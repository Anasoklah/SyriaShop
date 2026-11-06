using SyriaShop.Domain;
using SyriaShop.Application.Exceptions;

namespace SyriaShop.Application.Catalog;

public sealed class ProductService(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IProductService
{
    private const int MinPageSize = 1;
    private const int MaxPageSize = 100;

    public async Task<PagedResponse<ProductListItemResponse>> ListAsync(ProductListQuery query, CancellationToken cancellationToken = default)
    {
        if (query.Page < 1)
        {
            throw new DomainException("Page must be at least 1.");
        }

        if (query.PageSize is < MinPageSize or > MaxPageSize)
        {
            throw new DomainException($"Page size must be between {MinPageSize} and {MaxPageSize}.");
        }

        var result = await productRepository.ListAsync(query, cancellationToken);

        var categories = (await categoryRepository.GetByIdsAsync(result.Items.Select(p => p.CategoryId).ToList(), cancellationToken))
            .ToDictionary(c => c.Id);

        return new PagedResponse<ProductListItemResponse>(
            result.Items.Select(p => ToListItem(p, categories.GetValueOrDefault(p.CategoryId))).ToList(),
            result.Page,
            result.PageSize,
            result.TotalCount);
    }

    public async Task<ProductDetailResponse> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetBySlugAsync(slug, cancellationToken)
            ?? throw new NotFoundException($"Product with slug '{slug}' does not exist.");

        if (!product.IsActive)
        {
            throw new NotFoundException($"Product with slug '{slug}' is not available.");
        }

        return ToDetail(product, await categoryRepository.GetByIdAsync(product.CategoryId, cancellationToken));
    }

    public async Task<ProductDetailResponse> CreateAsync(ProductCreateRequest request, CancellationToken cancellationToken = default)
    {
        var category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new NotFoundException($"Category '{request.CategoryId}' does not exist.");

        var slug = await EnsureUniqueSlugAsync(request.Slug, request.NameEn, null, cancellationToken);

        var product = Product.Create(
            request.CategoryId,
            request.NameEn,
            request.NameAr,
            request.Price,
            request.Stock,
            slug,
            request.DescriptionEn,
            request.DescriptionAr,
            request.ImageUrl);

        productRepository.Add(product);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDetail(product, category);
    }

    public async Task<ProductDetailResponse> UpdateAsync(Guid id, ProductUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new NotFoundException($"Category '{request.CategoryId}' does not exist.");

        var product = await productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Product '{id}' does not exist.");

        if (!string.IsNullOrWhiteSpace(request.Slug))
        {
            var candidate = SlugGenerator.Generate(request.Slug.Trim());

            if (!candidate.Equals(product.Slug, StringComparison.Ordinal)
                && await productRepository.SlugExistsAsync(candidate, id, cancellationToken))
            {
                throw new ConflictException($"Product slug '{candidate}' is already used.");
            }

            product.ChangeSlug(candidate);
        }

        product.SetPrice(request.Price);
        product.SetStock(request.Stock);
        product.ChangeDetails(request.CategoryId, request.NameEn, request.NameAr,
            request.DescriptionEn, request.DescriptionAr, request.ImageUrl, request.IsActive);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDetail(product, category);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Product '{id}' does not exist.");

        productRepository.Remove(product);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<string> EnsureUniqueSlugAsync(string? requestedSlug, string fallbackSource, Guid? excludeId, CancellationToken cancellationToken)
    {
        var candidate = string.IsNullOrWhiteSpace(requestedSlug)
            ? SlugGenerator.Generate(fallbackSource)
            : SlugGenerator.Generate(requestedSlug.Trim());

        if (await productRepository.SlugExistsAsync(candidate, excludeId, cancellationToken))
        {
            throw new ConflictException($"Product slug '{candidate}' is already used.");
        }

        return candidate;
    }

    private static ProductListItemResponse ToListItem(Product product, Category? category) =>
        new(product.Id,
            category?.Slug ?? string.Empty,
            product.Slug,
            product.NameEn,
            product.NameAr,
            product.Price,
            product.ImageUrl,
            product.IsActive);

    private static ProductDetailResponse ToDetail(Product product, Category? category) =>
        new(product.Id,
            product.CategoryId,
            category?.Slug ?? string.Empty,
            category?.NameEn ?? string.Empty,
            category?.NameAr ?? string.Empty,
            product.Slug,
            product.NameEn,
            product.NameAr,
            product.DescriptionEn,
            product.DescriptionAr,
            product.Price,
            product.Stock,
            product.ImageUrl,
            product.IsActive,
            product.CreatedAt,
            product.UpdatedAt);
}
