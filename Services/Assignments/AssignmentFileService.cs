using Microsoft.Extensions.Options;
using OzelDersYonetim.Configuration;
using OzelDersYonetim.Services;

namespace OzelDersYonetim.Services.Assignments;

public class AssignmentFileService(StoragePathResolver storage, IOptions<FileUploadOptions> options)
{
    private static readonly HashSet<string> Extensions = new(StringComparer.OrdinalIgnoreCase) { ".pdf", ".jpg", ".jpeg", ".png", ".docx" };
    private static readonly HashSet<string> MimeTypes = new(StringComparer.OrdinalIgnoreCase) { "application/pdf", "image/jpeg", "image/png", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" };

    public async Task<string> SaveAsync(IFormFile file, int assignmentId, int? studentId = null)
    {
        var extension = Path.GetExtension(file.FileName);
        if (!Extensions.Contains(extension) || !MimeTypes.Contains(file.ContentType)) throw new InvalidOperationException("Yalnızca PDF, JPG, JPEG, PNG veya DOCX dosyaları yüklenebilir.");
        if (file.Length <= 0 || file.Length > options.Value.MaximumSizeMb * 1024L * 1024L) throw new InvalidOperationException($"Dosya boyutu en fazla {options.Value.MaximumSizeMb} MB olabilir.");
        var folder = studentId.HasValue
            ? storage.GetDirectory("assignments", assignmentId.ToString(), studentId.Value.ToString())
            : storage.GetDirectory("assignments", assignmentId.ToString(), "teacher");
        var safeName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        await using var stream = File.Create(Path.Combine(folder, safeName));
        await file.CopyToAsync(stream);
        return safeName;
    }

    public (Stream Stream, string ContentType)? Open(string storedName, int assignmentId, int? studentId = null)
    {
        var path = studentId.HasValue
            ? storage.ResolveStoredFile(storedName, "assignments", assignmentId.ToString(), studentId.Value.ToString())
            : storage.ResolveStoredFile(storedName, "assignments", assignmentId.ToString(), "teacher");
        if (path is null || !File.Exists(path)) return null;
        var contentType = Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".pdf" => "application/pdf",
            ".png" => "image/png",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "image/jpeg"
        };
        return (File.OpenRead(path), contentType);
    }
}
