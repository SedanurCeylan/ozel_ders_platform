using Microsoft.AspNetCore.Http;
using OzelDersYonetim.Models.Documents;

namespace OzelDersYonetim.Services.Documents;

public interface IStudentDocumentService
{
    Task CreateAsync(CourseDocument document, IFormFile file, IReadOnlyCollection<int> studentIds);
    Task<(Stream Stream, string ContentType, string FileName)?> OpenForStudentAsync(int documentId, string userId);
}
