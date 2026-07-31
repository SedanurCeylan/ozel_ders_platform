using OzelDersYonetim.Models.Students;

namespace OzelDersYonetim.Models.Documents;

public class StudentDocument
{
    public int Id { get; set; }
    public int StudentProfileId { get; set; }
    public StudentProfile StudentProfile { get; set; } = null!;
    public int CourseDocumentId { get; set; }
    public CourseDocument CourseDocument { get; set; } = null!;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public bool IsViewed { get; set; }
    public DateTime? ViewedAt { get; set; }
    public bool IsDownloaded { get; set; }
    public DateTime? DownloadedAt { get; set; }
}
