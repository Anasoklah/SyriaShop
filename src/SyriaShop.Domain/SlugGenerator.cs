namespace SyriaShop.Domain;

public static class SlugGenerator
{
    private const int MaxSlugLength = 200;

    public static string Generate(string source)
    {        var chars = source
            .ToLowerInvariant()
            .Select(c => char.IsAsciiLetterOrDigit(c) ? c : c is ' ' or '-' or '_' ? '-' : default(char?))
            .Where(c => c.HasValue)
            .Select(c => c!.Value)
            .ToArray();

        var slug = new string(chars);

        while (slug.Contains("--"))
        {
            slug = slug.Replace("--", "-");
        }

        var trimmed = slug.Trim('-');

        if (trimmed.Length == 0)
        {
            throw new DomainException("Slug source must contain at least one latin letter or digit.");
        }

        return trimmed.Length <= MaxSlugLength ? trimmed : trimmed[..MaxSlugLength];
    }
}
