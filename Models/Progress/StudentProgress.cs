using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OzelDersYonetim.Models.Students;

namespace OzelDersYonetim.Models.Progress;

public enum ProgressStatus { NotStarted, InProgress, Completed, NeedsImprovement }

public class StudentProgress
{
    public int Id { get; set; }
    public int StudentProfileId { get; set; }
    public StudentProfile StudentProfile { get; set; } = null!;
    [Required, StringLength(180), Display(Name = "Konu")] public string Topic { get; set; } = string.Empty;
    [Display(Name = "Gelişim durumu")] public ProgressStatus ProgressStatus { get; set; } = ProgressStatus.InProgress;
    [Range(0, 100), Display(Name = "Başarı yüzdesi")] public decimal SuccessPercentage { get; set; }
    [StringLength(2000), Display(Name = "Öğretmen yorumu")] public string? TeacherComment { get; set; }
    [Display(Name = "Değerlendirme tarihi")] public DateTime EvaluatedAt { get; set; } = DateTime.Now;
    [NotMapped, Display(Name = "Gelişim kaydını veliye e-posta ile bildir")] public bool NotifyParent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string StatusName => ProgressStatus switch { ProgressStatus.NotStarted => "Başlanmadı", ProgressStatus.InProgress => "Devam ediyor", ProgressStatus.Completed => "Tamamlandı", _ => "Geliştirilmeli" };
}
