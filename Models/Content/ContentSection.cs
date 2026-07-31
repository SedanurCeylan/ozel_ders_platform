using System.ComponentModel.DataAnnotations;

namespace OzelDersYonetim.Models.Content;

public class ContentSection
{
    public int Id { get; set; }

    [Required, StringLength(50), Display(Name = "Sayfa")]
    public string PageKey { get; set; } = "Home";

    [Required, StringLength(80), Display(Name = "Bölüm anahtarı")]
    public string SectionKey { get; set; } = string.Empty;

    [Required, StringLength(180), Display(Name = "Başlık")]
    public string Title { get; set; } = string.Empty;

    [StringLength(240), Display(Name = "Alt başlık")]
    public string? Subtitle { get; set; }

    [Required, StringLength(4000), Display(Name = "İçerik")]
    public string Content { get; set; } = string.Empty;

    [Display(Name = "Sıra")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Aktif")]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
