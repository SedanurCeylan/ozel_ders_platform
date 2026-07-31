using System.ComponentModel.DataAnnotations;

namespace OzelDersYonetim.Models.Assignments;

public class Assignment
{
    public int Id { get; set; }
    [Required, StringLength(180), Display(Name = "Ödev başlığı")] public string Title { get; set; } = string.Empty;
    [Required, StringLength(500), Display(Name = "Kısa açıklama")] public string ShortDescription { get; set; } = string.Empty;
    [StringLength(5000), Display(Name = "Detaylı açıklama")] public string? Description { get; set; }
    [Required, StringLength(120), Display(Name = "Ders veya konu")] public string Subject { get; set; } = "Matematik";
    [StringLength(40), Display(Name = "Sınıf seviyesi")] public string? GradeLevel { get; set; }
    [Display(Name = "Başlangıç tarihi")] public DateTime StartDate { get; set; } = DateTime.Now;
    [Display(Name = "Son teslim tarihi")] public DateTime DueDate { get; set; } = DateTime.Now.AddDays(7);
    [Range(1, 1000), Display(Name = "Maksimum puan")] public int MaximumScore { get; set; } = 100;
    [StringLength(500)] public string? AttachmentPath { get; set; }
    [Display(Name = "Ödev durumu")] public AssignmentStatus Status { get; set; } = AssignmentStatus.Published;
    [Display(Name = "Geç teslim kabul edilsin")] public bool AllowLateSubmission { get; set; }
    [Display(Name = "Öğrenci dosya yükleyebilsin")] public bool AllowFileUpload { get; set; } = true;
    [Display(Name = "Öğrenci açıklama yazabilsin")] public bool AllowStudentComment { get; set; } = true;
    [Display(Name = "E-posta bildirimi gönderilsin")] public bool SendEmailNotification { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public ICollection<StudentAssignment> StudentAssignments { get; set; } = new List<StudentAssignment>();
}
