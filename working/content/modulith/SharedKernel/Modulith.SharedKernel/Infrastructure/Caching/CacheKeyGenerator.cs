namespace Modulith.SharedKernel.Infrastructure.Caching;

public static class CacheKeyGenerator
{
    public static string GenerateKey(string prefix, params object[] parts)
    {
        var keyParts = new List<string> { prefix };
        keyParts.AddRange(parts.Select(p => p?.ToString()?.ToLowerInvariant() ?? "null"));
        return string.Join(":", keyParts);
    }

    public static string GenerateKey<T>(string prefix, T entity) where T : class
    {
        var typeName = typeof(T).Name.ToLowerInvariant();
        var id = entity.GetType().GetProperty("Id")?.GetValue(entity)?.ToString()?.ToLowerInvariant() ?? "unknown";
        return GenerateKey(prefix, typeName, id);
    }

    public static string GenerateListKey<T>(string prefix) where T : class
    {
        var typeName = typeof(T).Name.ToLowerInvariant();
        return GenerateKey(prefix, typeName, "list");
    }

    public static string GenerateFilteredListKey<T>(string prefix, object filter) where T : class
    {
        var typeName = typeof(T).Name.ToLowerInvariant();
        var filterHash = filter.GetHashCode().ToString("x");
        return GenerateKey(prefix, typeName, "filtered", filterHash);
    }
} 