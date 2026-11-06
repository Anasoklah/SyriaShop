using SyriaShop.Api;

namespace SyriaShop.UnitTests;

public class EnvFileLoaderTests
{
    private static string WriteEnvFile(params string[] lines)
    {
        var path = Path.Combine(Path.GetTempPath(), $"env-{Guid.NewGuid():N}.env");
        File.WriteAllLines(path, lines);

        return path;
    }

    [Fact]
    public void Load_ParsesKeyValuePairs()
    {
        var path = WriteEnvFile("ConnectionStrings__Default=Server=ANAS-OKLAH\\ANAS_O;Database=SyriaShopDb;");

        var settings = EnvFileLoader.Load(path);

        Assert.Equal("Server=ANAS-OKLAH\\ANAS_O;Database=SyriaShopDb;", settings["ConnectionStrings:Default"]);
    }

    [Fact]
    public void Load_TranslatesDoubleUnderscoresToHierarchySeparator()
    {
        var path = WriteEnvFile("Section__Nested__Key=value");

        var settings = EnvFileLoader.Load(path);

        Assert.Equal("value", settings["Section:Nested:Key"]);
        Assert.False(settings.ContainsKey("Section__Nested__Key"));
    }

    [Fact]
    public void Load_SkipsCommentsAndBlankLines()
    {
        var path = WriteEnvFile("# a comment", "", "   ", "KEY=value");

        var settings = EnvFileLoader.Load(path);

        var item = Assert.Single(settings);
        Assert.Equal("KEY", item.Key);
        Assert.Equal("value", item.Value);
    }

    [Fact]
    public void Load_KeepsExtraEqualsSignsInValue()
    {
        var path = WriteEnvFile("Formula=a=b=c");

        var settings = EnvFileLoader.Load(path);

        Assert.Equal("a=b=c", settings["Formula"]);
    }

    [Fact]
    public void Load_TrimsWhitespaceAroundKeyAndValue()
    {
        var path = WriteEnvFile("   KEY   =   value   ");

        var settings = EnvFileLoader.Load(path);

        Assert.Equal("value", settings["KEY"]);
    }

    [Fact]
    public void Load_ReturnsEmptyWhenFileDoesNotExist()
    {
        var settings = EnvFileLoader.Load(Path.Combine(Path.GetTempPath(), "does-not-exist.env"));

        Assert.Empty(settings);
    }

    [Fact]
    public void Load_TreatsLineWithoutSeparatorAsInvalid()
    {
        var path = WriteEnvFile("BROKEN_LINE", "KEY=value");

        var settings = EnvFileLoader.Load(path);

        var item = Assert.Single(settings);
        Assert.Equal("KEY", item.Key);
    }
}
