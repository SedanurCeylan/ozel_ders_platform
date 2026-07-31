using System.ComponentModel.DataAnnotations;

namespace OzelDersYonetim.Models.Assignments;

public class AssignmentSubmission
{
    public int Id { get; set; }
    public int StudentAssignmentId { get; set; }
    public StudentAssignment StudentAssignment { get; set; } = null!;
    [StringLength(3000)] public string? StudentDescription { get; set; }
    [StringLength(255)] public string? FileName { get; set; }
    [StringLength(500)] public string? FilePath { get; set; }
    public long? FileSize { get; set; }
    public int SubmissionNumber { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
