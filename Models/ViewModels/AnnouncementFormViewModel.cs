using Microsoft.AspNetCore.Mvc.Rendering;
using OzelDersYonetim.Models.Notifications;
using Microsoft.AspNetCore.Http;
namespace OzelDersYonetim.Models.ViewModels;
public class AnnouncementFormViewModel { public Announcement Announcement { get; set; } = new(); public List<int> SelectedStudentIds { get; set; } = new(); public IReadOnlyList<SelectListItem> Students { get; set; } = Array.Empty<SelectListItem>(); public IFormFile? Attachment { get; set; } }
