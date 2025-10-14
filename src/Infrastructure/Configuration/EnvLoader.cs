namespace Infrastructure.Configuration;

public static class EnvLoader
{
    public static IDictionary<string, string> Load(string filePath)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (!File.Exists(filePath))
        {
            return values;
        }

        foreach (var rawLine in File.ReadAllLines(filePath))
        {
            var line = rawLine.Trim();

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            var separatorIndex = line.IndexOf('=', StringComparison.Ordinal);
            if (separatorIndex <= 0)
            {
                continue;
            }

            var key = line[..separatorIndex].Trim();
            var value = line[(separatorIndex + 1)..].Trim();

            if (value.StartsWith('"') && value.EndsWith('"') && value.Length >= 2)
            {
                value = value[1..^1];
            }

            values[key] = value;
        }

        return values;
    }

    public static void Apply(IDictionary<string, string> values)
    {
        foreach (var pair in values)
        {
            Environment.SetEnvironmentVariable(pair.Key, pair.Value);
        }
    }

    public static string GetRequired(this IDictionary<string, string> source, string key)
    {
        if (source.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        throw new InvalidOperationException($"Не найдено обязательное значение переменной окружения: {key}");
    }

    public static string? GetOptional(this IDictionary<string, string> source, string key, string? defaultValue = null)
    {
        return source.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : defaultValue;
    }
}
