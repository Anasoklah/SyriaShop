using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SyriaShop.Infrastructure;

public sealed class AppDbContextDesignFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private const string FallbackConnectionString =
        "Server=localhost;Database=SyriaShopDb;Trusted_Connection=True;TrustServerCertificate=True;";

    public AppDbContext CreateDbContext(string[] args)
    {
        return new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(ResolveConnectionString(), sql =>
                sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName))
            .Options);
    }

    private static string ResolveConnectionString()
    {
        var directory = AppContext.BaseDirectory;

        for (var i = 0; i < 8 && directory is not null; i++)
        {
            var envFilePath = Path.Combine(directory, ".env");
            if (File.Exists(envFilePath))
            {
                foreach (var line in File.ReadAllLines(envFilePath))
                {
                    var index = line.IndexOf('=');
                    if (index <= 0 || !line[..index].Trim().Equals("ConnectionStrings__Default", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    var value = line[(index + 1)..].Trim();
                    if (value.Length > 0)
                    {
                        return value;
                    }
                }
            }

            directory = Path.GetDirectoryName(directory);
        }

        return FallbackConnectionString;
    }
}
