using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OzelDersYonetim.Models.Students;

namespace OzelDersYonetim.Models.Progress;

public class ExamResult
{
    public int Id { get; set; }
    public int StudentProfileId { get; set; }
    public StudentProfile StudentProfile { get; set; } = null!;
    [Required, StringLength(160), Display(Name = "Sınav adı")] public string ExamName { get; set; } = string.Empty;
    [Display(Name = "Sınav tarihi")] public DateTime ExamDate { get; set; } = DateTime.Now.Date;
    [Range(1, 1000), Display(Name = "Toplam soru")] public int TotalQuestions { get; set; }
    [Range(0, 1000), Display(Name = "Doğru")] public int CorrectCount { get; set; }
    [Range(0, 1000), Display(Name = "Yanlış")] public int WrongCount { get; set; }
    [Range(0, 1000), Display(Name = "Boş")] public int EmptyCount { get; set; }
    [Display(Name = "Net")] public decimal NetScore { get; set; }
    [Range(0, 1000), Display(Name = "Matematik puanı")] public decimal? MathematicsScore { get; set; }
    [Range(1, 1000), Display(Name = "Süre (dakika)")] public int? DurationMinutes { get; set; }
    [StringLength(2000), Display(Name = "Öğretmen yorumu")] public string? TeacherComment { get; set; }
    [StringLength(500)] public string? ResultFilePath { get; set; }
    [NotMapped, Display(Name = "Sonucu veliye e-posta ile bildir")] public bool NotifyParent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public void CalculateNet(decimal wrongPenalty = 4m) => NetScore = CorrectCount - WrongCount / wrongPenalty;
}
