using System.ComponentModel.DataAnnotations;

namespace OzelDersYonetim.Models.Content;

public class DailyFact
{
    public int Id { get; set; }
    [Required,StringLength(180),Display(Name="Bilgi başlığı")] public string Title { get; set; }=string.Empty;
    [Required,StringLength(1200),Display(Name="İlginç bilgi")] public string Content { get; set; }=string.Empty;
    [Required,StringLength(80),Display(Name="Kategori")] public string Category { get; set; }="Genel Matematik";
    [Display(Name="Sıra")] public int DisplayOrder { get; set; }
    [Display(Name="Aktif")] public bool IsActive { get; set; }=true;
    public DateTime CreatedAt { get; set; }=DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }=DateTime.UtcNow;
}
