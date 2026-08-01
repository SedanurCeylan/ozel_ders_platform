using System.ComponentModel.DataAnnotations;

namespace OzelDersYonetim.Models.Content;

public class SiteSetting
{
    public int Id { get; set; }

    [Required, StringLength(120), Display(Name = "Site adı")]
    public string SiteName { get; set; } = "Matematik Atölyesi";

    [Required, StringLength(120), Display(Name = "Öğretmen adı")]
    public string TeacherName { get; set; } = "Sena Öğretmen";

    [EmailAddress, StringLength(180), Display(Name = "İletişim e-posta adresi")]
    public string? Email { get; set; }

    [Phone, StringLength(40), Display(Name = "Telefon")]
    public string? Phone { get; set; }

    [Url, StringLength(300), Display(Name = "Instagram profil bağlantısı")]
    public string? InstagramUrl { get; set; }

    [StringLength(120), Display(Name = "EmailJS Service ID")]
    public string? EmailJsServiceId { get; set; }

    [StringLength(120), Display(Name = "EmailJS Template ID")]
    public string? EmailJsTemplateId { get; set; }

    [StringLength(180), Display(Name = "EmailJS Public Key")]
    public string? EmailJsPublicKey { get; set; }

    [Required, StringLength(180), Display(Name = "Ana sayfa başlığı")]
    public string HeroTitle { get; set; } = "Matematik ezber değil, anlama yolculuğudur.";

    [Required, StringLength(500), Display(Name = "Ana sayfa açıklaması")]
    public string HeroDescription { get; set; } = "Her öğrencinin hızına, hedeflerine ve öğrenme biçimine göre şekillenen derslerle matematiği birlikte anlaşılır hâle getiriyoruz.";

    [Required, StringLength(180), Display(Name = "Hakkımda başlığı")]
    public string AboutTitle { get; set; } = "Her öğrenci matematiği anlayabilir.";

    [Required, StringLength(1200), Display(Name = "Hakkımda açıklaması")]
    public string AboutDescription { get; set; } = "Doğru anlatım, sabır ve kişiye özel bir yol haritasıyla matematik; kaygı kaynağı olmaktan çıkıp güçlü bir düşünme aracına dönüşür.";

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
