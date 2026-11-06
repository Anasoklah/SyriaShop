using SyriaShop.Domain;

namespace SyriaShop.UnitTests;

public class ProductTests
{
    private static readonly Guid CategoryId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_IsActiveWithGeneratedSlug()
    {
        var product = Product.Create(CategoryId, "Premium Olive Oil", "زيت زيتون ممتاز", price: 160000m, stock: 70);

        Assert.Equal("premium-olive-oil", product.Slug);
        Assert.True(product.IsActive);
        Assert.Equal(CategoryId, product.CategoryId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1_000_000)]
    public void Create_WithInvalidPrice_ThrowsDomainException(decimal price)
    {
        Assert.Throws<DomainException>(() => Product.Create(CategoryId, "Tea", "شاي", price, stock: 10));
    }

    [Fact]
    public void Create_WithNegativeStock_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => Product.Create(CategoryId, "Tea", "شاي", price: 100, stock: -1));
    }

    [Fact]
    public void SetStock_ToZero_SucceedsAndKeepsProductSellable()
    {
        var product = Product.Create(CategoryId, "Serum", "سيروم", price: 52000m, stock: 90);

        product.SetStock(0);

        Assert.Equal(0, product.Stock);
        Assert.True(product.IsActive);
    }

    [Fact]
    public void SetPrice_UpdateChangesPriceOnly()
    {
        var product = Product.Create(CategoryId, "Serum", "سيروم", price: 52000m, stock: 90);

        product.SetPrice(54000m);

        Assert.Equal(54000m, product.Price);
        Assert.Equal(90, product.Stock);
    }

    [Fact]
    public void ChangeDetails_WithTooLongDescription_ThrowsDomainException()
    {
        var product = Product.Create(CategoryId, "Serum", "سيروم", price: 52000m, stock: 90);

        Assert.Throws<DomainException>(() =>
            product.ChangeDetails(CategoryId, "Serum", "سيروم", new string('x', 2001), null, null, true));
    }

    [Fact]
    public void ChangeCategory_MovesProductBetweenCategories()
    {
        var product = Product.Create(CategoryId, "Tea", "شاي", price: 45000m, stock: 10);
        var newCategoryId = Guid.NewGuid();

        product.ChangeCategory(newCategoryId);

        Assert.Equal(newCategoryId, product.CategoryId);
    }

    [Fact]
    public void ChangeSlug_NormalizesProvidedSlug()
    {
        var product = Product.Create(CategoryId, "Serum", "سيروم", price: 52000m, stock: 90);

        product.ChangeSlug("  Vitamin -- C Serum ");

        Assert.Equal("vitamin-c-serum", product.Slug);
    }
}
