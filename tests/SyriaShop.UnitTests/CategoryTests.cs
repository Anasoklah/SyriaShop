using SyriaShop.Domain;

namespace SyriaShop.UnitTests;

public class CategoryTests
{
    [Fact]
    public void Create_WithValidNames_GeneratesSlugFromEnglishName()
    {
        var category = Category.Create("Home & Kitchen", "المنزل والمطبخ");

        Assert.Equal("home-kitchen", category.Slug);
        Assert.NotEqual(Guid.Empty, category.Id);
    }

    [Fact]
    public void Create_WithExplicitSlug_UsesProvidedSlug()
    {
        var category = Category.Create("Electronics", "الإلكترونيات", "tech-devices");

        Assert.Equal("tech-devices", category.Slug);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyEnglishName_ThrowsDomainException(string nameEn)
    {
        Assert.Throws<DomainException>(() => Category.Create(nameEn, "الإلكترونيات"));
    }

    [Fact]
    public void ChangeDetails_WithEmptyArabicName_ThrowsDomainException()
    {
        var category = Category.Create("Books", "الكتب");

        Assert.Throws<DomainException>(() => category.ChangeDetails("Books", "  ", isActive: true));
    }

    [Fact]
    public void ChangeSlug_NormalizesInput()
    {
        var category = Category.Create("Books", "الكتب");

        category.ChangeSlug("  Family  Books -- 2026 ");

        Assert.Equal("family-books-2026", category.Slug);
    }
}
