namespace SyriaShop.Domain;

public sealed class Category : Entity
{
    private const int NameMaxLength = 200;

    private Category()
    {
    }

    private Category(string slug, string nameEn, string nameAr)
    {
        Slug = slug;
        NameEn = nameEn;
        NameAr = nameAr;
        IsActive = true;
    }

    public string Slug { get; private set; } = null!;

    public string NameEn { get; private set; } = null!;

    public string NameAr { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public static Category Create(string nameEn, string nameAr, string? slug = null)
    {
        ChangeNameGuard(ref nameEn, ref nameAr);

        return new Category(SlugGenerator.Generate(slug ?? nameEn), nameEn, nameAr);
    }

    public void ChangeDetails(string nameEn, string nameAr, bool isActive)
    {
        ChangeNameGuard(ref nameEn, ref nameAr);

        NameEn = nameEn;
        NameAr = nameAr;
        IsActive = isActive;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ChangeSlug(string slug)
    {
        var normalized = SlugGenerator.Generate(slug);

        Slug = normalized;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static void ChangeNameGuard(ref string nameEn, ref string nameAr)
    {
        nameEn = nameEn.Trim();
        nameAr = nameAr.Trim();

        if (nameEn.Length is 0 or > NameMaxLength)
        {
            throw new DomainException($"English name must be between 1 and {NameMaxLength} characters and not only whitespace.");
        }

        if (nameAr.Length is 0 or > NameMaxLength)
        {
            throw new DomainException($"Arabic name must be between 1 and {NameMaxLength} characters and not only whitespace.");
        }
    }
}
