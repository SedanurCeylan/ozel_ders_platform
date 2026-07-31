using System.ComponentModel.DataAnnotations;
namespace OzelDersYonetim.Models.Notifications;
public class EmailLog
{
    public int Id { get; set; }
    [Required, StringLength(180)] public string RecipientEmail { get; set; } = string.Empty;
    [StringLength(180)] public string? RecipientName { get; set; }
    [Required, StringLength(250)] public string Subject { get; set; } = string.Empty;
    [Required, StringLength(80)] public string EmailType { get; set; } = string.Empty;
    public int? RelatedEntityId { get; set; }
    [Required] public string Body { get; set; } = string.Empty;
    public bool IsSuccessful { get; set; }
    [StringLength(2000)] public string? ErrorMessage { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
