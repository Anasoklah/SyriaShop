namespace SyriaShop.Api;

public static class EnvFileLocator
{
    public static string? Locate()
    {
        var directory = AppContext.BaseDirectory;

        for (var depth = 0; depth < 8 && directory is not null; depth++)
        {
            var direct = Path.Combine(directory, ".env");

            if (File.Exists(direct))
            {
                return direct;
            }

            var sourceLayout = Path.Combine(directory, "src", "SyriaShop.Api", ".env");

            if (File.Exists(sourceLayout))
            {
                return sourceLayout;
            }

            directory = Path.GetDirectoryName(directory);
        }

        return null;
    }
}
