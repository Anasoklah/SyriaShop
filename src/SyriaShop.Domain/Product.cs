namespace SyriaShop.Domain;

public sealed class Product : Entity
{
    private const int NameMaxLength = 200;
    private const int DescriptionMaxLength = 2000;
    private const int ImageUrlMaxLength = 500;

    private Product()
    {
    }

    private Product(Guid categoryId, string slug, string nameEn, string nameAr, string? descriptionEn,
        string? descriptionAr, decimal price, int stock, string? imageUrl)
    {
        CategoryId = categoryId;
        Slug = slug;
        NameEn = nameEn;
        NameAr = nameAr;
        DescriptionEn = descriptionEn;
        DescriptionAr = descriptionAr;
        Price = price;
        Stock = stock;
        ImageUrl = imageUrl;
        IsActive = true;
    }

    public Guid CategoryId { get; private set; }

    public string Slug { get; private set; } = null!;

    public string NameEn { get; private set; } = null!;

    public string NameAr { get; private set; } = null!;

    public string? DescriptionEn { get; private set; }

    public string? DescriptionAr { get; private set; }

    public decimal Price { get; private set; }

    public int Stock { get; private set; }

    public string? ImageUrl { get; private set; }

    public bool IsActive { get; private set; }

    public static Product Create(Guid categoryId, string nameEn, string nameAr, decimal price, int stock,
        string? slug = null, string? descriptionEn = null, string? descriptionAr = null, string? imageUrl = null)
    {
        var product = new Product(
            categoryId,
            SlugGenerator.Generate(slug ?? nameEn),
            nameEn,
            nameAr,
            descriptionEn,
            descriptionAr,
            price,
            stock,
            imageUrl);

        SetPriceGuard(product, price);
        SetStockGuard(stock);

        return product;
    }

    public void SetPrice(decimal price)
    {
        SetPriceGuard(this, price);

        Price = price;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SetStock(int stock)
    {
        SetStockGuard(stock);

        Stock = stock;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ChangeDetails(Guid categoryId, string nameEn, string nameAr, string? descriptionEn,
        string? descriptionAr, string? imageUrl, bool isActive)
    {
        if (nameEn.Trim().Length is 0 or > NameMaxLength)
        {
            throw new DomainException($"English name must be between 1 and {NameMaxLength} characters and not only whitespace.");
        }

        if (nameAr.Trim().Length is 0 or > NameMaxLength)
        {
            throw new DomainException($"Arabic name must be between 1 and {NameMaxLength} characters and not only whitespace.");
        }

        ValidateText(descriptionEn, nameof(descriptionEn), DescriptionMaxLength);
        ValidateText(descriptionAr, nameof(descriptionAr), DescriptionMaxLength);
        ValidateText(imageUrl, nameof(imageUrl), ImageUrlMaxLength);

        CategoryId = categoryId;
        NameEn = nameEn.Trim();
        NameAr = nameAr.Trim();
        DescriptionEn = descriptionEn;
        DescriptionAr = descriptionAr;
        ImageUrl = imageUrl;
        IsActive = isActive;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ChangeCategory(Guid categoryId)
    {
        CategoryId = categoryId;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ChangeSlug(string slug)
    {
        Slug = SlugGenerator.Generate(slug);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static void SetPriceGuard(Product product, decimal price)
    {
        if (price is <= 0 or > 999_999)
        {
            throw new DomainException("Price must be positive and at most 999999.");
        }
    }

    private static void SetStockGuard(int stock)
    {
        if (stock < 0)
        {
            throw new DomainException("Stock cannot be negative.");
        }
    }

    private static void ValidateText(string? value, string field, int maxLength)
    {
        if (value is not null && value.Trim().Length > maxLength)
        {
            throw new DomainException($"{field} must be at most {maxLength} characters.");
        }
    }
}
