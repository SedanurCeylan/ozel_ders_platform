using System.ComponentModel.DataAnnotations;
using OzelDersYonetim.Models.Identity;
namespace OzelDersYonetim.Models.Notifications;
public class UserNotification
{
    public int Id { get; set; }
    [Required] public string ApplicationUserId { get; set; } = string.Empty;
    public ApplicationUser ApplicationUser { get; set; } = null!;
    [Required, StringLength(180)] public string Title { get; set; } = string.Empty;
    [Required, StringLength(1000)] public string Message { get; set; } = string.Empty;
    [Required, StringLength(80)] public string NotificationType { get; set; } = string.Empty;
    public int? RelatedEntityId { get; set; }
    [StringLength(500)] public string? TargetUrl { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; set; }
}
