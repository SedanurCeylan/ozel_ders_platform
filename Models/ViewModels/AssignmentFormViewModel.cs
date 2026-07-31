using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using OzelDersYonetim.Models.Assignments;

namespace OzelDersYonetim.Models.ViewModels;

public class AssignmentFormViewModel
{
    public Assignment Assignment { get; set; } = new();
    public List<int> SelectedStudentIds { get; set; } = new();
    public IFormFile? Attachment { get; set; }
    public IReadOnlyList<SelectListItem> Students { get; set; } = Array.Empty<SelectListItem>();
}

public class AssignmentSubmissionViewModel
{
    public StudentAssignment StudentAssignment { get; set; } = null!;
    public string? StudentDescription { get; set; }
    public IFormFile? File { get; set; }
}
