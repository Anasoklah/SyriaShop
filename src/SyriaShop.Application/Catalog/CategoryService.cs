using SyriaShop.Domain;
using SyriaShop.Application.Exceptions;

namespace SyriaShop.Application.Catalog;

public sealed class CategoryService(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : ICategoryService
{
    private const int MinPageSize = 1;
    private const int MaxPageSize = 100;

    public async Task<PagedResponse<CategoryDetailResponse>> ListAsync(int page, int pageSize, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        GuardPaging(page, pageSize);

        var collection = await categoryRepository.ListAsync(page, pageSize, includeInactive, cancellationToken);

        return new PagedResponse<CategoryDetailResponse>(
            collection.Items.Select(ToDetail).ToList(),
            collection.Page,
            collection.PageSize,
            collection.TotalCount);
    }

    public async Task<CategoryDetailResponse> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var category = await categoryRepository.GetBySlugAsync(slug, cancellationToken)
            ?? throw new NotFoundException($"Category with slug '{slug}' does not exist.");

        return ToDetail(category);
    }

    public async Task<CategoryDetailResponse> CreateAsync(CategoryCreateRequest request, CancellationToken cancellationToken = default)
    {
        var slug = await EnsureUniqueSlugAsync(request.Slug, request.NameEn, null, cancellationToken);

        var category = Category.Create(request.NameEn, request.NameAr, slug);

        categoryRepository.Add(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDetail(category);
    }

    public async Task<CategoryDetailResponse> UpdateAsync(Guid id, CategoryUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var category = await categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Category '{id}' does not exist.");

        if (!string.IsNullOrWhiteSpace(request.Slug))
        {
            var candidate = SlugGenerator.Generate(request.Slug.Trim());

            if (!candidate.Equals(category.Slug, StringComparison.Ordinal))
            {
                if (await categoryRepository.SlugExistsAsync(candidate, id, cancellationToken))
                {
                    throw new ConflictException($"Category slug '{candidate}' is already used.");
                }

                category.ChangeSlug(candidate);
            }
        }

        category.ChangeDetails(request.NameEn, request.NameAr, request.IsActive);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDetail(category);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Category '{id}' does not exist.");

        var productCount = await categoryRepository.CountProductsAsync(id, cancellationToken);

        if (productCount > 0)
        {
            throw new ConflictException($"Category '{category.Slug}' still contains {productCount} product(s). Move or delete them first.");
        }

        categoryRepository.Remove(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static void GuardPaging(int page, int pageSize)
    {
        if (page < 1)
        {
            throw new DomainException("Page must be at least 1.");
        }

        if (pageSize is < MinPageSize or > MaxPageSize)
        {
            throw new DomainException($"Page size must be between {MinPageSize} and {MaxPageSize}.");
        }
    }

    private async Task<string?> EnsureUniqueSlugAsync(string? requestedSlug, string fallbackSource, Guid? excludeId, CancellationToken cancellationToken)
    {
        var candidate = string.IsNullOrWhiteSpace(requestedSlug)
            ? SlugGenerator.Generate(fallbackSource)
            : SlugGenerator.Generate(requestedSlug.Trim());

        if (await categoryRepository.SlugExistsAsync(candidate, excludeId, cancellationToken))
        {
            throw new ConflictException($"Category slug '{candidate}' is already used.");
        }

        return candidate;
    }

    private static CategoryDetailResponse ToDetail(Category category) =>
        new(category.Id, category.Slug, category.NameEn, category.NameAr, category.IsActive, category.CreatedAt);
}
