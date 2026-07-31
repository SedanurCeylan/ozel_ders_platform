using System.ComponentModel.DataAnnotations;

namespace OzelDersYonetim.Models.ViewModels;

public class TeacherProfileViewModel
{
    [Required, StringLength(120), Display(Name = "Öğretmenin adı ve soyadı")]
    public string TeacherName { get; set; } = string.Empty;

    [Required, StringLength(180), Display(Name = "Mesleki unvan")]
    public string ProfessionalTitle { get; set; } = string.Empty;

    [Required, StringLength(500), Display(Name = "Eğitim bilgisi")]
    public string Education { get; set; } = string.Empty;

    [Required, StringLength(500), Display(Name = "Deneyim ve uzmanlık")]
    public string Experience { get; set; } = string.Empty;

    [Required, StringLength(500), Display(Name = "Ders biçimi")]
    public string LessonFormat { get; set; } = string.Empty;

    [Required, StringLength(180), Display(Name = "Hakkımda sayfası başlığı")]
    public string AboutTitle { get; set; } = string.Empty;

    [Required, StringLength(1200), Display(Name = "Kısa tanıtım yazısı")]
    public string AboutDescription { get; set; } = string.Empty;

    [Required, StringLength(4000), Display(Name = "Öğretmen sözü")]
    public string Quote { get; set; } = string.Empty;

    [Required, StringLength(180), Display(Name = "Eğitim yaklaşımı başlığı")]
    public string ApproachTitle { get; set; } = string.Empty;

    [StringLength(240), Display(Name = "Yaklaşım kısa başlığı")]
    public string? ApproachSubtitle { get; set; }

    [Required, StringLength(4000), Display(Name = "Eğitim yaklaşımı açıklaması")]
    public string ApproachContent { get; set; } = string.Empty;

    [Required, StringLength(180), Display(Name = "Birinci değer")]
    public string FirstValueTitle { get; set; } = string.Empty;
    [Required, StringLength(4000), Display(Name = "Birinci değer açıklaması")]
    public string FirstValueContent { get; set; } = string.Empty;

    [Required, StringLength(180), Display(Name = "İkinci değer")]
    public string SecondValueTitle { get; set; } = string.Empty;
    [Required, StringLength(4000), Display(Name = "İkinci değer açıklaması")]
    public string SecondValueContent { get; set; } = string.Empty;

    [Required, StringLength(180), Display(Name = "Üçüncü değer")]
    public string ThirdValueTitle { get; set; } = string.Empty;
    [Required, StringLength(4000), Display(Name = "Üçüncü değer açıklaması")]
    public string ThirdValueContent { get; set; } = string.Empty;
}
