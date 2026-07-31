using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using OzelDersYonetim.Models.Documents;

namespace OzelDersYonetim.Models.ViewModels;

public class DocumentFormViewModel
{
    public CourseDocument Document { get; set; } = new();
    public IFormFile? File { get; set; }
    public List<int> SelectedStudentIds { get; set; } = new();
    public IReadOnlyList<SelectListItem> Students { get; set; } = Array.Empty<SelectListItem>();
}
