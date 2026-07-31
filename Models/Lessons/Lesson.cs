using System.ComponentModel.DataAnnotations;
namespace OzelDersYonetim.Models.Lessons;
public class Lesson
{
    public int Id { get; set; }
    [Required, StringLength(160), Display(Name="Ders başlığı")] public string Title { get; set; } = string.Empty;
    [StringLength(2000), Display(Name="Açıklama")] public string? Description { get; set; }
    [Required, StringLength(120), Display(Name="Ders / konu")] public string Subject { get; set; } = "Matematik";
    [Display(Name="Başlangıç")] public DateTime StartDate { get; set; }
    [Display(Name="Bitiş")] public DateTime EndDate { get; set; }
    [Display(Name="Ders biçimi")] public LessonMode LessonMode { get; set; }
    [Url, StringLength(500), Display(Name="Online ders bağlantısı")] public string? OnlineMeetingUrl { get; set; }
    [Display(Name="Ders durumu")] public LessonStatus Status { get; set; } = LessonStatus.Planned;
    [Display(Name="Öğrenciye e-posta bildirimi gönder")] public bool SendEmailNotification { get; set; }
    [StringLength(2000), Display(Name="Öğretmen notu")] public string? TeacherNote { get; set; }
    [StringLength(500), Display(Name="Sonraki ders konusu")] public string? NextLessonTopic { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public ICollection<StudentLesson> StudentLessons { get; set; } = new List<StudentLesson>();
}
