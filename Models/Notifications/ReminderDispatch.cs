using System.ComponentModel.DataAnnotations;
namespace OzelDersYonetim.Models.Notifications;
public class ReminderDispatch
{
    public int Id { get; set; }
    [Required,StringLength(80)] public string ReminderType { get; set; }=string.Empty;
    [Required,StringLength(80)] public string EntityType { get; set; }=string.Empty;
    public int EntityId { get; set; }
    [Required] public string ApplicationUserId { get; set; }=string.Empty;
    public DateTime CreatedAt { get; set; }=DateTime.UtcNow;
}
