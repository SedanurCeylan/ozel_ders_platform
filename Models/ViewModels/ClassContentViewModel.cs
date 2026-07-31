using System.ComponentModel.DataAnnotations;

namespace OzelDersYonetim.Models.ViewModels;

public class ClassContentViewModel
{
    public int Id { get; set; }
    [Required, StringLength(80), Display(Name="Sınıf etiketi")] public string GradeLabel { get; set; } = string.Empty;
    [Required, StringLength(180), Display(Name="Sayfa başlığı")] public string Title { get; set; } = string.Empty;
    [Required, StringLength(4000), Display(Name="Konular")] public string Topics { get; set; } = string.Empty;
    [Display(Name="Görüntüleme sırası")] public int DisplayOrder { get; set; }
    [Display(Name="Sitede göster")] public bool IsActive { get; set; } = true;
}
