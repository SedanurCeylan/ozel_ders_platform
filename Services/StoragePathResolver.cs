using Microsoft.Extensions.Options;
using OzelDersYonetim.Configuration;

namespace OzelDersYonetim.Services;

public sealed class StoragePathResolver(IWebHostEnvironment environment, IOptions<StorageOptions> options)
{
    public string GetDirectory(params string[] segments)
    {
        var configuredRoot = options.Value.RootPath.Trim();
        var root = Path.IsPathRooted(configuredRoot)
            ? configuredRoot
            : Path.Combine(environment.ContentRootPath, configuredRoot);
        var path = segments.Aggregate(root, Path.Combine);
        Directory.CreateDirectory(path);
        return path;
    }

    public string? ResolveStoredFile(string storedName, params string[] segments)
    {
        var safeName = Path.GetFileName(storedName);
        if (string.IsNullOrWhiteSpace(safeName) || !string.Equals(safeName, storedName, StringComparison.Ordinal)) return null;
        return Path.Combine(GetDirectory(segments), safeName);
    }
}
