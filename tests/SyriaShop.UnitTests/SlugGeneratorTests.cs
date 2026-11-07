using SyriaShop.Domain;

namespace SyriaShop.UnitTests;

public class SlugGeneratorTests
{
    [Theory]
    [InlineData("Home & Kitchen", "home-kitchen")]
    [InlineData("Beauty  &   Care ", "beauty-care")]
    [InlineData("الإلكترونيات", "")]
    [InlineData("Stationery_2026", "stationery-2026")]
    [InlineData("---", "")]
    public void Generate_NormalizesSourcesAsExpected(string source, string expected)
    {
        if (expected.Length == 0)
        {
            Assert.Throws<DomainException>(() => SlugGenerator.Generate(source));
        }
        else
        {
            Assert.Equal(expected, SlugGenerator.Generate(source));
        }
    }

    [Fact]
    public void Generate_TruncatesSourcesLongerThan200Characters()
    {
        var source = new string('a', 500);

        var slug = SlugGenerator.Generate(source);

        Assert.Equal(200, slug.Length);
    }
}
