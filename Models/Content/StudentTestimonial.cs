using System.ComponentModel.DataAnnotations;
using OzelDersYonetim.Models.Students;
namespace OzelDersYonetim.Models.Content;
public class StudentTestimonial
{
    public int Id { get; set; }
    public int StudentProfileId { get; set; }
    public StudentProfile StudentProfile { get; set; } = null!;
    [Required, StringLength(1000), Display(Name="Yorumunuz")] public string Comment { get; set; }=string.Empty;
    [Range(1,5), Display(Name="Puanınız")] public int Rating { get; set; }=5;
    [Display(Name="Yayında")] public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }=DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }=DateTime.UtcNow;
    public string PublicStudentName=>$"{StudentProfile.FirstName} {(string.IsNullOrWhiteSpace(StudentProfile.LastName)?"":StudentProfile.LastName[0]+".")}";
}
