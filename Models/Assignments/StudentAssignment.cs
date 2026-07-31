using System.ComponentModel.DataAnnotations;
using OzelDersYonetim.Models.Students;

namespace OzelDersYonetim.Models.Assignments;

public class StudentAssignment
{
    public int Id { get; set; }
    public int AssignmentId { get; set; }
    public Assignment Assignment { get; set; } = null!;
    public int StudentProfileId { get; set; }
    public StudentProfile StudentProfile { get; set; } = null!;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public DateTime? ViewedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public StudentAssignmentStatus Status { get; set; } = StudentAssignmentStatus.Assigned;
    [Range(0, 1000)] public decimal? Score { get; set; }
    [StringLength(3000)] public string? TeacherFeedback { get; set; }
    public bool IsLate { get; set; }
    public DateTime? EvaluatedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public ICollection<AssignmentSubmission> Submissions { get; set; } = new List<AssignmentSubmission>();
}
