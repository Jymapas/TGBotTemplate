using System;

namespace Infrastructure.Configuration;

public static class EnvLoader
{
    public static IDictionary<string, string> Load(string filePath)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var resolvedPath = ResolvePath(filePath);
        if (resolvedPath is null)
        {
            return values;
        }

        foreach (var rawLine in File.ReadAllLines(resolvedPath))
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
        if (TryGetCombinedValue(source, key, out var value))
        {
            return value;
        }

        throw new InvalidOperationException($"Не найдено обязательное значение переменной окружения: {key}");
    }

    public static string? GetOptional(this IDictionary<string, string> source, string key, string? defaultValue = null)
    {
        return TryGetCombinedValue(source, key, out var value)
            ? value
            : defaultValue;
    }

    private static bool TryGetCombinedValue(IDictionary<string, string> source, string key, out string value)
    {
        if (source.TryGetValue(key, out var fileValue) && !string.IsNullOrWhiteSpace(fileValue))
        {
            value = fileValue;
            return true;
        }

        var envValue = Environment.GetEnvironmentVariable(key);
        if (!string.IsNullOrWhiteSpace(envValue))
        {
            value = envValue;
            return true;
        }

        value = string.Empty;
        return false;
    }

    private static string? ResolvePath(string filePath)
    {
        if (File.Exists(filePath))
        {
            return filePath;
        }

        var fileName = Path.GetFileName(filePath);
        if (string.IsNullOrEmpty(fileName))
        {
            return null;
        }

        var directories = new List<string>();

        var explicitDirectory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(explicitDirectory))
        {
            directories.Add(Path.GetFullPath(explicitDirectory));
        }

        directories.Add(Directory.GetCurrentDirectory());
        directories.Add(AppContext.BaseDirectory);

        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var directory in directories)
        {
            if (!visited.Add(directory))
            {
                continue;
            }

            var current = new DirectoryInfo(directory);
            while (current is not null)
            {
                var candidate = Path.Combine(current.FullName, fileName);
                if (File.Exists(candidate))
                {
                    return candidate;
                }

                current = current.Parent;
            }
        }

        return null;
    }
}
