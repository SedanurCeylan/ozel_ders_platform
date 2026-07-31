using Microsoft.Extensions.Options;
using OzelDersYonetim.Configuration;

namespace OzelDersYonetim.Services.Assignments;

public class AssignmentFileService(IWebHostEnvironment environment, IOptions<FileUploadOptions> options)
{
    private static readonly HashSet<string> Extensions = new(StringComparer.OrdinalIgnoreCase) { ".pdf", ".jpg", ".jpeg", ".png", ".docx" };
    private static readonly HashSet<string> MimeTypes = new(StringComparer.OrdinalIgnoreCase) { "application/pdf", "image/jpeg", "image/png", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" };

    public async Task<string> SaveAsync(IFormFile file, int assignmentId, int? studentId = null)
    {
        var extension = Path.GetExtension(file.FileName);
        if (!Extensions.Contains(extension) || !MimeTypes.Contains(file.ContentType)) throw new InvalidOperationException("Yalnızca PDF, JPG, JPEG, PNG veya DOCX dosyaları yüklenebilir.");
        if (file.Length <= 0 || file.Length > options.Value.MaximumSizeMb * 1024L * 1024L) throw new InvalidOperationException($"Dosya boyutu en fazla {options.Value.MaximumSizeMb} MB olabilir.");
        var relativeFolder = studentId.HasValue ? Path.Combine("uploads", "assignments", assignmentId.ToString(), studentId.Value.ToString()) : Path.Combine("uploads", "assignments", assignmentId.ToString(), "teacher");
        var folder = Path.Combine(environment.WebRootPath, relativeFolder);
        Directory.CreateDirectory(folder);
        var safeName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        await using var stream = File.Create(Path.Combine(folder, safeName));
        await file.CopyToAsync(stream);
        return "/" + Path.Combine(relativeFolder, safeName).Replace('\\', '/');
    }
}
