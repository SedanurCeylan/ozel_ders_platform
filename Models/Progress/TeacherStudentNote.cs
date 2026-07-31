using System.ComponentModel.DataAnnotations;
using OzelDersYonetim.Models.Students;

namespace OzelDersYonetim.Models.Progress;

public enum TeacherNoteCategory { General, Academic, Assignment, Attendance, ParentMeeting, Progress, Reminder }

public class TeacherStudentNote
{
    public int Id { get; set; }
    public int StudentProfileId { get; set; }
    public StudentProfile StudentProfile { get; set; } = null!;
    [Required, StringLength(180), Display(Name = "Başlık")] public string Title { get; set; } = string.Empty;
    [Required, StringLength(4000), Display(Name = "Açıklama")] public string Description { get; set; } = string.Empty;
    [Display(Name = "Kategori")] public TeacherNoteCategory Category { get; set; }
    [Display(Name = "Önemli")] public bool IsImportant { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string CategoryName => Category switch { TeacherNoteCategory.General => "Genel", TeacherNoteCategory.Academic => "Akademik", TeacherNoteCategory.Assignment => "Ödev", TeacherNoteCategory.Attendance => "Katılım", TeacherNoteCategory.ParentMeeting => "Veli görüşmesi", TeacherNoteCategory.Progress => "Gelişim", _ => "Hatırlatma" };
}
