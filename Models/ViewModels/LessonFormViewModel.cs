using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using OzelDersYonetim.Models.Lessons;
namespace OzelDersYonetim.Models.ViewModels;
public class LessonFormViewModel
{
    public Lesson Lesson { get; set; } = new() { StartDate=DateTime.Now.AddDays(1).Date.AddHours(17), EndDate=DateTime.Now.AddDays(1).Date.AddHours(18) };
    [Display(Name="Öğrenciler")] public List<int> SelectedStudentIds { get; set; } = new();
    public IReadOnlyList<SelectListItem> Students { get; set; } = Array.Empty<SelectListItem>();
}
