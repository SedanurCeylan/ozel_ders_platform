using System.ComponentModel.DataAnnotations;
using OzelDersYonetim.Models.Students;
namespace OzelDersYonetim.Models.Lessons;
public class StudentLesson
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;
    public int StudentProfileId { get; set; }
    public StudentProfile StudentProfile { get; set; } = null!;
    [Display(Name="Katılım durumu")] public AttendanceStatus AttendanceStatus { get; set; } = AttendanceStatus.Pending;
    [StringLength(1000), Display(Name="Performans notu")] public string? PerformanceNote { get; set; }
    [Range(0,1000), Display(Name="Çözülen soru")] public int? QuestionCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
