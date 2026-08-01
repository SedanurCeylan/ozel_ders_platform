using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OzelDersYonetim.Configuration;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Documents;
using OzelDersYonetim.Services;

namespace OzelDersYonetim.Services.Documents;

public class StudentDocumentService(ApplicationDbContext dbContext, StoragePathResolver storage, IOptions<FileUploadOptions> options) : IStudentDocumentService
{
    private static readonly HashSet<string> Extensions = new(StringComparer.OrdinalIgnoreCase) { ".pdf", ".jpg", ".jpeg", ".png", ".docx" };
    private static readonly HashSet<string> MimeTypes = new(StringComparer.OrdinalIgnoreCase) { "application/pdf", "image/jpeg", "image/png", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" };

    public async Task CreateAsync(CourseDocument document, IFormFile file, IReadOnlyCollection<int> studentIds)
    {
        var extension = Path.GetExtension(file.FileName);
        if (!Extensions.Contains(extension) || !MimeTypes.Contains(file.ContentType)) throw new InvalidOperationException("Yalnızca PDF, JPG, JPEG, PNG veya DOCX dosyaları yüklenebilir.");
        if (file.Length <= 0 || file.Length > options.Value.MaximumSizeMb * 1024L * 1024L) throw new InvalidOperationException($"Dosya boyutu en fazla {options.Value.MaximumSizeMb} MB olabilir.");
        if (document.AccessType == DocumentAccessType.SelectedStudents && studentIds.Count == 0) throw new InvalidOperationException("Özel paylaşım için en az bir öğrenci seçin.");
        var folder = storage.GetDirectory("documents");
        var storedName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        await using (var stream = File.Create(Path.Combine(folder, storedName))) await file.CopyToAsync(stream);
        document.OriginalFileName = Path.GetFileName(file.FileName); document.StoredFilePath = storedName; document.ContentType = file.ContentType; document.FileSize = file.Length; document.CreatedAt = DateTime.UtcNow;
        dbContext.CourseDocuments.Add(document); await dbContext.SaveChangesAsync();
        if (document.AccessType == DocumentAccessType.SelectedStudents) foreach (var id in studentIds.Distinct()) dbContext.StudentDocuments.Add(new StudentDocument { CourseDocumentId = document.Id, StudentProfileId = id });
        await dbContext.SaveChangesAsync();
    }

    public async Task<(Stream Stream, string ContentType, string FileName)?> OpenForStudentAsync(int documentId, string userId)
    {
        var student = await dbContext.StudentProfiles.SingleOrDefaultAsync(x => x.ApplicationUserId == userId && x.IsActive); if (student is null) return null;
        var document = await dbContext.CourseDocuments.Include(x => x.StudentDocuments).SingleOrDefaultAsync(x => x.Id == documentId && x.IsActive); if (document is null) return null;
        var allowed = document.AccessType != DocumentAccessType.SelectedStudents || document.StudentDocuments.Any(x => x.StudentProfileId == student.Id); if (!allowed) return null;
        var tracking = document.StudentDocuments.SingleOrDefault(x => x.StudentProfileId == student.Id);
        if (tracking is null) { tracking = new StudentDocument { CourseDocumentId = document.Id, StudentProfileId = student.Id }; dbContext.StudentDocuments.Add(tracking); }
        tracking.IsViewed = tracking.IsDownloaded = true; tracking.ViewedAt ??= DateTime.UtcNow; tracking.DownloadedAt = DateTime.UtcNow; await dbContext.SaveChangesAsync();
        var path = storage.ResolveStoredFile(document.StoredFilePath, "documents"); if (path is null || !File.Exists(path)) return null;
        return (File.OpenRead(path), document.ContentType, document.OriginalFileName);
    }
}
