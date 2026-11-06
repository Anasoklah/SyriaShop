namespace SyriaShop.Api;

public static class EnvFileLoader
{
    public static Dictionary<string, string?> Load(string filePath)
    {
        var settings = new Dictionary<string, string?>();

        if (!File.Exists(filePath))
        {
            return settings;
        }

        foreach (var line in File.ReadAllLines(filePath))
        {
            var trimmed = line.Trim();

            if (trimmed.Length == 0 || trimmed.StartsWith('#'))
            {
                continue;
            }

            var separatorIndex = trimmed.IndexOf('=');
            if (separatorIndex <= 0)
            {
                continue;
            }

            var key = trimmed[..separatorIndex].Trim();
            var value = trimmed[(separatorIndex + 1)..].Trim();

            settings[key.Replace("__", ":")] = value;
        }

        return settings;
    }
}
