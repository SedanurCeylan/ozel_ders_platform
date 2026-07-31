using System.ComponentModel.DataAnnotations;
using OzelDersYonetim.Models.Students;
namespace OzelDersYonetim.Models.Notifications;
public enum AnnouncementTargetType { AllStudents, GradeLevel, SelectedStudents }
public class Announcement
{
    public int Id { get; set; }
    [Required, StringLength(180), Display(Name="Başlık")] public string Title { get; set; } = string.Empty;
    [Required, StringLength(6000), Display(Name="İçerik")] public string Content { get; set; } = string.Empty;
    [Display(Name="Hedef türü")] public AnnouncementTargetType TargetType { get; set; }
    [StringLength(40), Display(Name="Hedef sınıf")] public string? GradeLevel { get; set; }
    [StringLength(500)] public string? AttachmentPath { get; set; }
    [Display(Name="Yayın tarihi")] public DateTime PublishDate { get; set; } = DateTime.Now;
    [Display(Name="Bitiş tarihi")] public DateTime? ExpiryDate { get; set; }
    [Display(Name="E-posta gönder")] public bool SendEmail { get; set; }
    [Display(Name="Aktif")] public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public ICollection<AnnouncementStudent> AnnouncementStudents { get; set; } = new List<AnnouncementStudent>();
    public string TargetName => TargetType switch { AnnouncementTargetType.AllStudents => "Tüm öğrenciler", AnnouncementTargetType.GradeLevel => GradeLevel ?? "Sınıf", _ => "Seçili öğrenciler" };
}
public class AnnouncementStudent
{
    public int Id { get; set; }
    public int AnnouncementId { get; set; }
    public Announcement Announcement { get; set; } = null!;
    public int StudentProfileId { get; set; }
    public StudentProfile StudentProfile { get; set; } = null!;
    public bool IsViewed { get; set; }
    public DateTime? ViewedAt { get; set; }
}
